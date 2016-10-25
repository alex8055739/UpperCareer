using AVOSCloud;
using RTCareerAsk.DAL.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTCareerAsk.DAL
{
    public class LeanCloudAccess
    {
        #region Upper

        #region User Operation
        /// <summary>
        /// 注册一个新用户并存入数据库。
        /// </summary>
        /// <param name="u">新用户信息模型</param>
        /// <returns>代表注册是否成功的Boolean值</returns>
        public async Task<bool> RegisterUser(User u)
        {
            bool isSuccess = false;
            AVUser uo = u.CreateUserObjectForRegister();

            await uo.SignUpAsync().ContinueWith(t =>
                {
                    if (t.IsCanceled || t.IsFaulted)
                    {
                        throw t.Exception;
                    }

                    isSuccess = true;
                });

            if (isSuccess)
            {
                isSuccess = false;

                List<string> roleNames = new List<string>();

                foreach (Role r in u.Roles)
                {
                    roleNames.Add(r.RoleName);
                }

                return await AssignRolesToUser(uo.ObjectId, roleNames);
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 验证用户是否存在。
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>代表用户是否存在的Boolean值</returns>
        public async Task<bool> ValidateUser(string userName, string password)
        {
            bool IsValidated = false;

            try
            {
                await AVUser.LogInAsync(userName, password).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }
                    else
                    {
                        IsValidated = true;
                    }
                });

                return IsValidated;
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// 通过电子邮箱和密码登陆用户，并返回用户信息。
        /// </summary>
        /// <param name="email">邮箱地址</param>
        /// <param name="password">密码</param>
        /// <returns>返回的用户信息</returns>
        public async Task<User> LoginWithEmail(string email, string password)
        {
            return await AVUser.LogInByEmailAsync(email, password).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }
                    else
                    {
                        return AVRole.Query.WhereEqualTo("users", t.Result).FindAsync().ContinueWith(s => new User(t.Result).SetRoles(s.Result));
                    }
                }).Unwrap();
        }
        /// <summary>
        /// 通过电子邮件帮助用户重置密码。
        /// </summary>
        /// <param name="email">邮箱地址</param>
        /// <returns>代表操作是否成功的Boolean值</returns>
        public async Task<bool> ResetPassword(string email)
        {
            return await AVUser.RequestPasswordResetAsync(email).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    return true;
                });
        }
        /// <summary>
        /// 更换用户头像。
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="portraitUrl">头像图片的URL</param>
        /// <returns>代表操作是否成功的Boolean值</returns>
        public async Task<bool> ChangeUserPortrait(string userId, string portraitUrl)
        {
            AVUser u = await AVUser.Query.GetAsync(userId);

            //Delete old portrait file if existed.
            if (u.ContainsKey("portrait") && !string.IsNullOrEmpty(u.Get<string>("portrait")))
            {
                await AVObject.GetQuery("_File").WhereEqualTo("url", u.Get<string>("portrait")).FirstAsync().ContinueWith(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                        {
                            throw t.Exception;
                        }

                        t.Result.DeleteAsync().ContinueWith(s =>
                            {
                                if (s.IsFaulted || s.IsCanceled)
                                {
                                    throw s.Exception;
                                }
                            });
                    });
            }

            u["portrait"] = portraitUrl;

            return await u.SaveAsync().ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }

                return true;
            });
        }
        /// <summary>
        /// 保存更新用户详细信息。
        /// </summary>
        /// <param name="ud">用户详细信息模型</param>
        /// <returns>代表保存更新是否成功的Boolean值</returns>
        public async Task<bool> SaveUserDetail(UserDetail ud)
        {
            if (string.IsNullOrEmpty(ud.ForUser.ObjectID) || string.IsNullOrEmpty(ud.ForUser.Name))
            {
                throw new InvalidOperationException("没有指定保存对象ID或称谓空缺");
            }

            if (!string.IsNullOrEmpty(ud.ObjectId))
            {
                return await AVObject.GetQuery("UserDetail").GetAsync(ud.ObjectId).ContinueWith(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                        {
                            throw t.Exception;
                        }

                        return ud.UpdateUserDetailObject(t.Result).SaveAsync().ContinueWith(u =>
                        {
                            if (u.IsFaulted || u.IsCanceled)
                            {
                                throw u.Exception;
                            }

                            return AVUser.Query.GetAsync(ud.ForUser.ObjectID).ContinueWith(v =>
                            {
                                if (v.IsFaulted || v.IsCanceled)
                                {
                                    throw v.Exception;
                                }

                                v.Result["nickname"] = ud.ForUser.Name;
                                v.Result["title"] = ud.ForUser.Title;
                                v.Result["gender"] = ud.ForUser.Gender;
                                v.Result["company"] = ud.ForUser.Company;
                                v.Result["fieldIndex"] = ud.ForUser.FieldIndex;

                                return v.Result.SaveAsync().ContinueWith(w =>
                                    {
                                        if (w.IsFaulted || w.IsCanceled)
                                        {
                                            throw w.Exception;
                                        }

                                        return true;
                                    });
                            });
                        });
                    }).Unwrap().Unwrap().Unwrap();
            }

            return await ud.CreateUserDetailObjectForSave().SaveAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    return AVUser.Query.GetAsync(ud.ForUser.ObjectID).ContinueWith(s =>
                        {
                            if (s.IsFaulted || s.IsCanceled)
                            {
                                throw s.Exception;
                            }

                            s.Result["nickname"] = ud.ForUser.Name;
                            return s.Result.SaveAsync().ContinueWith(x =>
                                {
                                    if (x.IsFaulted || x.IsCanceled)
                                    {
                                        throw x.Exception;
                                    }

                                    return true;
                                });
                        });
                }).Unwrap().Unwrap();
        }
        /// <summary>
        /// 关注一个用户。
        /// </summary>
        /// <param name="userId">关注者ID</param>
        /// <param name="followeeId">关注对象ID</param>
        /// <returns>代表关注是否成功的Boolean值</returns>
        public async Task<bool> Follow(string userId, string followeeId)
        {
            return await AVUser.Query.GetAsync(userId).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    return t.Result.Follow(followeeId).ContinueWith(s =>
                        {
                            if (s.IsFaulted || s.IsCanceled)
                            {
                                throw s.Exception;
                            }

                            return s.Result;
                        });
                }).Unwrap();
        }
        /// <summary>
        /// 取消一个关注。
        /// </summary>
        /// <param name="userId">关注者ID</param>
        /// <param name="followeeId">关注对象ID</param>
        /// <returns>代表取关是否成功的Boolean值</returns>
        public async Task<bool> Unfollow(string userId, string followeeId)
        {
            return await AVUser.Query.GetAsync(userId).ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }

                return t.Result.Unfollow(followeeId);
            }).Unwrap();
        }
        /// <summary>
        /// 读取用户的粉丝。
        /// </summary>
        /// <param name="userId">用户</param>
        /// <returns>所有粉丝信息</returns>
        public async Task<IEnumerable<User>> GetFollowers(string userId)
        {
            return await AVUser.Query.GetAsync(userId).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    return t.Result.GetFollowers().ContinueWith(s =>
                    {
                        if (s.IsFaulted || s.IsCanceled)
                        {
                            throw s.Exception;
                        }

                        List<User> followers = new List<User>();

                        foreach (AVUser follower in s.Result)
                        {
                            followers.Add(new User(follower.Get<AVUser>("follower")));
                        }

                        return followers;
                    });
                }).Unwrap();
        }
        /// <summary>
        /// 读取用户的关注。
        /// </summary>
        /// <param name="userId">用户</param>
        /// <returns>所有关注对象信息</returns>
        public async Task<IEnumerable<User>> GetFollowees(string userId)
        {
            return await AVUser.Query.GetAsync(userId).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    return t.Result.GetFolloweeQuery().FindAsync().ContinueWith(s =>
                    {
                        if (s.IsFaulted || s.IsCanceled)
                        {
                            throw s.Exception;
                        }

                        List<User> followees = new List<User>();

                        foreach (AVUser followee in s.Result)
                        {
                            followees.Add(new User(followee.Get<AVUser>("followee")));
                        }

                        return followees;
                    });
                }).Unwrap();
        }
        /// <summary>
        /// 给一个用户指派身份。
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleNames">获得的新身份名集合</param>
        /// <returns>代表指派是否成功的Boolean值</returns>
        public async Task<bool> AssignRolesToUser(string userId, IEnumerable<string> roleNames)
        {
            AVUser u = AVUser.CreateWithoutData("_User", userId) as AVUser;
            List<Task<AVRole>> rs = new List<Task<AVRole>>();

            foreach (string roleName in roleNames)
            {
                rs.Add(AVRole.Query.Include("users").WhereEqualTo("name", roleName).FirstAsync().ContinueWith(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                        {
                            throw t.Exception;
                        }

                        t.Result.Get<AVRelation<AVUser>>("users").Add(u);

                        return t.Result;
                    }));
            }

            await Task.WhenAll(rs.ToArray());

            #region Code for Debug
            //foreach (Task<AVRole> r in rs)
            //{
            //    await r.Result.SaveAsync().ContinueWith(t =>
            //        {
            //            if (t.IsFaulted || t.IsCanceled)
            //            {
            //                throw t.Exception;
            //            }
            //        });
            //}
            #endregion

            List<Task> ts = new List<Task>();

            foreach (Task<AVRole> tr in rs)
            {
                ts.Add(tr.Result.SaveAsync().ContinueWith(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                        {
                            throw t.Exception;
                        }
                    }));
            }

            await Task.WhenAll(ts.ToArray());

            return true;
        }
        /// <summary>
        /// 给一个用户去除身份。
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleNames">需要去除身份名集合</param>
        /// <returns>代表去除是否成功的Boolean值</returns>
        public async Task<bool> RemoveRolesFromUser(string userId, IEnumerable<string> roleNames)
        {
            AVUser u = AVUser.CreateWithoutData("_User", userId) as AVUser;
            List<Task<AVRole>> rs = new List<Task<AVRole>>();

            foreach (string roleName in roleNames)
            {
                rs.Add(AVRole.Query.Include("users").WhereEqualTo("name", roleName).FirstAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    t.Result.Get<AVRelation<AVUser>>("users").Remove(u);

                    return t.Result;
                }));
            }

            await Task.WhenAll(rs.ToArray());

            #region Code for Debug
            //foreach (Task<AVRole> r in rs)
            //{
            //    await r.Result.SaveAsync().ContinueWith(t =>
            //        {
            //            if (t.IsFaulted || t.IsCanceled)
            //            {
            //                throw t.Exception;
            //            }
            //        });
            //}
            #endregion

            List<Task> ts = new List<Task>();

            foreach (Task<AVRole> tr in rs)
            {
                ts.Add(tr.Result.SaveAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }
                }));
            }

            await Task.WhenAll(ts.ToArray());

            return true;
        }
        /// <summary>
        /// 封禁一个用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>代表封禁是否成功的Boolean值</returns>
        public async Task<bool> BlockUser(string userId)
        {
            string blockRoleId = "578d09bf0a2b580068522e52";

            AVUser u = AVUser.CreateWithoutData("_User", userId) as AVUser;

            AVRole r = await AVRole.Query.Include("users").GetAsync(blockRoleId).ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }
                else
                {
                    t.Result.Get<AVRelation<AVUser>>("users").Add(u);
                    return t.Result;
                }
            });

            await r.SaveAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }
                });

            return true;
        }
        /// <summary>
        /// 解除封禁一个用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>代表解封是否成功的Boolean值</returns>
        public async Task<bool> UnblockUser(string userId)
        {
            string blockRoleId = "578d09bf0a2b580068522e52";

            AVUser u = AVUser.CreateWithoutData("_User", userId) as AVUser;

            AVRole r = await AVRole.Query.Include("users").GetAsync(blockRoleId).ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }
                else
                {
                    t.Result.Get<AVRelation<AVUser>>("users").Remove(u);
                    return t.Result;
                }
            });

            await r.SaveAsync().ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }
            });

            return true;
        }
        #endregion

        #region User Detail Load
        /// <summary>
        /// 读取一条用户信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户信息</returns>
        public async Task<User> LoadUserInfo(string userId)
        {
            return await AVUser.Query.GetAsync(userId).ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }

                return new User(t.Result);
            });
        }
        /// <summary>
        /// 查阅一名用户的详细信息。
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>包含用户详细信息的Object</returns>
        public async Task<UserDetail> LoadUserDetail(string userId)
        {
            return await AVObject.GetQuery("UserDetail")
                .Include("forUser")
                .WhereEqualTo("forUser", AVUser.CreateWithoutData("_User", userId) as AVUser)
                .FindAsync()
                .ContinueWith(async t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    if (t.Result.Count() == 0)
                    {
                        return new UserDetail(await GetUserByID(userId).ContinueWith(s =>
                                {
                                    if (s.IsFaulted || s.IsCanceled)
                                    {
                                        throw s.Exception;
                                    }

                                    return s.Result;
                                }));
                    }

                    return new UserDetail(t.Result.First());
                }).Unwrap();
        }
        /// <summary>
        /// 查询是否已经关注了目标用户。
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="targetId">目标用户ID</param>
        /// <returns>代表是否已关注目标用户的Boolean值</returns>
        public async Task<bool> IfAlreadyFollowed(string userId, string targetId)
        {
            AVUser user = AVUser.CreateWithoutData("_User", userId) as AVUser;
            AVUser target = AVUser.CreateWithoutData("_User", targetId) as AVUser;

            AVQuery<AVObject> query = AVObject.GetQuery("_Followee").WhereEqualTo("user", user).WhereEqualTo("followee", target);

            return await query.FindAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    return t.Result.Count() > 0 ? true : false;
                });
        }
        /// <summary>
        /// 查询用户的粉丝数。
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>粉丝数量</returns>
        public async Task<int> GetFollowerCount(string userId)
        {
            return await AVUser.Query.GetAsync(userId).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    return t.Result.GetFollowerQuery().CountAsync().ContinueWith(s =>
                        {
                            if (s.IsFaulted || s.IsCanceled)
                            {
                                throw s.Exception;
                            }

                            return s.Result;
                        });
                }).Unwrap();
        }
        /// <summary>
        /// 查询用户的关注数。
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>关注数量</returns>
        public async Task<int> GetFolloweeCount(string userId)
        {
            return await AVUser.Query.GetAsync(userId).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    return t.Result.GetFolloweeQuery().CountAsync().ContinueWith(s =>
                        {
                            if (s.IsFaulted || s.IsCanceled)
                            {
                                throw s.Exception;
                            }

                            return s.Result;
                        });
                }).Unwrap();
        }
        /// <summary>
        /// 查询用户发布的近10条问题。
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>10条用户最近发布的问题内容</returns>
        public async Task<List<QuestionInfo>> GetRecentQuestions(string userId)
        {
            AVUser u = AVUser.CreateWithoutData("_User", userId) as AVUser;

            return await AVObject.GetQuery("Post").WhereEqualTo("createdBy", u).Limit(10).OrderByDescending("createdAt").FindAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    List<QuestionInfo> questions = new List<QuestionInfo>();

                    foreach (AVObject ans in t.Result)
                    {
                        questions.Add(new QuestionInfo(ans));
                    }

                    return questions;
                });
        }
        /// <summary>
        /// 查询用户发布的近10条回答。
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>10条用户最近发布的答案内容</returns>
        public async Task<List<Answer>> GetRecentAnswers(string userId)
        {
            AVUser u = AVUser.CreateWithoutData("_User", userId) as AVUser;

            return await AVObject.GetQuery("Answer").Include("forQuestion").WhereEqualTo("createdBy", u).Limit(10).OrderByDescending("createdAt").FindAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    List<Answer> answers = new List<Answer>();

                    foreach (AVObject ans in t.Result)
                    {
                        answers.Add(new Answer(ans));
                    }

                    return answers;
                });
        }
        #endregion

        #region Invitation Code Operation
        /// <summary>
        /// 生成一列新的邀请码并存在数据库。
        /// </summary>
        /// <param name="isMaster">是否为超级邀请码</param>
        /// <param name="pre">一列邀请码后缀</param>
        /// <returns>一列已生成的邀请码</returns>
        public async Task<IEnumerable<string>> GenerateNewInviteCode(bool isMaster, IEnumerable<string> pre)
        {
            List<string> sl = new List<string>();
            List<Task<string>> tl = new List<Task<string>>();

            foreach (string s in pre)
            {
                tl.Add(GenerateNewInviteCode(isMaster, s));
            }

            await Task.WhenAll(tl.ToArray());

            foreach (Task<string> t in tl)
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }

                sl.Add(t.Result);
            }

            return sl;
        }
        /// <summary>
        /// 生成一条新的邀请码并存在数据库。
        /// </summary>
        /// <param name="isMaster">是否为超级邀请码</param>
        /// <param name="s">邀请码后缀</param>
        /// <returns>已生成的邀请码；若是邀请码已存在或操作失误，返回一条空值</returns>
        public async Task<string> GenerateNewInviteCode(bool isMaster, string s)
        {
            string code = string.Concat("UPPER", s);

            if (await IsInviteCodeExisted(code))
            {
                return null;
            }

            AVObject inviteCode = AVObject.Create("Invitation_Code");

            inviteCode.Add("inviteCode", s);
            inviteCode.Add("isValid", true);
            inviteCode.Add("isMaster", isMaster);

            await inviteCode.SaveAsync().ContinueWith(t =>
            {
                if (t.IsCanceled || t.IsFaulted)
                {
                    throw t.Exception;
                }
            });

            return code;
        }
        /// <summary>
        /// 验证一条邀请码是否已经存在过。
        /// </summary>
        /// <param name="code">新生成的邀请码</param>
        /// <returns>表示是否存在相同邀请码的Boolean值</returns>
        public async Task<bool> IsInviteCodeExisted(string code)
        {
            return await AVObject.GetQuery("Invitation_Code").WhereEqualTo("inviteCode", code).CountAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    return t.Result > 0;
                });
        }
        /// <summary>
        /// 验证一条邀请码是否有效。
        /// </summary>
        /// <param name="code">邀请码</param>
        /// <returns>表示邀请码是否有效的Boolean值</returns>
        public async Task<bool> IsInviteCodeValid(string code)
        {
            return await AVObject.GetQuery("Invitation_Code").WhereEqualTo("inviteCode", code).WhereEqualTo("isValid", true).CountAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    return t.Result > 0;
                });
        }
        /// <summary>
        /// 查找一条邀请码的相关数据。
        /// </summary>
        /// <param name="code">邀请码</param>
        /// <returns>若找到对应数据，返回一个邀请码类Object，反之返回空值</returns>
        public async Task<InvitationCode> FindInviteCode(string code)
        {
            return await AVObject.GetQuery("Invitation_Code").WhereEqualTo("inviteCode", code).FindAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    return t.Result.Count() > 0 ? new InvitationCode(t.Result.First()) : null;
                });
        }
        /// <summary>
        /// 废弃一条邀请码。
        /// </summary>
        /// <param name="code">邀请码</param>
        /// <returns>表示邀请码是否成功废弃的Boolean值</returns>
        public async Task<bool> DisposeInviteCode(string code)
        {
            AVObject c = await AVObject.GetQuery("Invitation_Code").WhereEqualTo("inviteCode", code).FindAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    return t.Result.Count() > 0 ? t.Result.First() : null;
                });

            if (c == null)
            {
                return false;
            }

            if (!Convert.ToBoolean(c["isMaster"]))
            {
                c["isValid"] = false;

                await c.SaveAsync().ContinueWith(t =>
                {
                    if (t.IsCanceled || t.IsFaulted)
                    {
                        throw t.Exception;
                    }
                });
            }

            return true;
        }
        #endregion

        #region Question List Load
        /// <summary>
        /// 读取一页问题列表。
        /// </summary>
        /// <param name="pageIndex">选择跳过多少页</param>
        /// <param name="isHottestFirst">是否按照热度降序排列，否则按照新鲜度排列</param>
        /// <returns>经过筛选和排序的一页问题列表</returns>
        public async Task<IEnumerable<QuestionInfo>> LoadQuestionList(int pageIndex, bool isHottestFirst)
        {
            int pageItemCount = 20;

            return await AVObject.GetQuery("Post")
                .Include("createdBy")
                .Skip(pageIndex * pageItemCount)
                .Limit(pageItemCount)
                .OrderByDescending(isHottestFirst ? "subPostCount" : "createdAt")
                .FindAsync()
                .ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    List<QuestionInfo> questions = new List<QuestionInfo>();

                    foreach (AVObject obj in t.Result)
                    {
                        questions.Add(new QuestionInfo(obj));
                    }

                    return questions;
                });
        }
        /// <summary>
        /// 读取一页答案列表。
        /// </summary>
        /// <param name="pageIndex">选择跳过多少页</param>
        /// <param name="isHottestFirst">是否按照热度降序排列，否则按照新鲜度排列</param>
        /// <returns>经过筛选和排序的一页答案列表</returns>
        public async Task<IEnumerable<AnswerInfo>> LoadAnswerList(int pageIndex, bool isHottestFirst)
        {
            int pageItemCount = 20;

            return await AVObject.GetQuery("Answer")
                .Include("forQuestion")
                .Include("createdBy")
                .Skip(pageIndex * pageItemCount)
                .Limit(pageItemCount)
                .OrderByDescending(isHottestFirst ? "subPostCount" : "createdAt")
                .FindAsync()
                .ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    List<AnswerInfo> answers = new List<AnswerInfo>();

                    foreach (AVObject obj in t.Result)
                    {
                        answers.Add(new AnswerInfo(obj));
                    }

                    return answers;
                });

        }
        #endregion

        #region Question Detail Load
        /// <summary>
        /// 根据问题ID查找问题详情及答案。
        /// </summary>
        /// <param name="userId">提出请求的用户ID</param>
        /// <param name="questionId">问题ID</param>
        /// <returns>包含所有答案的问题详情</returns>
        public async Task<Question> GetQuestionAndAnswersWithComments(string userId, string questionId)
        {
            try
            {
                return await FindAnswersByQuestion(userId, questionId).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    return AVObject.GetQuery("Post").Include("createdBy").GetAsync(questionId).ContinueWith(s =>
                    {
                        if (s.IsFaulted || s.IsCanceled)
                        {
                            throw s.Exception;
                        }

                        UpdateViewCount(s.Result);

                        return IsQuestionLikedByUser(userId, s.Result).ContinueWith(r =>
                            {
                                return new Question(s.Result, t.Result).SetVote(r.Result);
                            });
                    });
                }).Unwrap().Unwrap();
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 通过问题ID查找答案。
        /// </summary>
        /// <param name="userId">提出请求的用户ID</param>
        /// <param name="questionId">问题ID</param>
        /// <returns>问题下所有回答</returns>
        public async Task<IEnumerable<Answer>> FindAnswersByQuestion(string userId, string questionId)
        {
            try
            {
                return await AVObject.GetQuery("Answer")
                    .Include("createdBy")
                    .WhereEqualTo("forQuestion", AVObject.CreateWithoutData("Post", questionId))
                    .FindAsync()
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                        {
                            throw t.Exception;
                        }

                        return GetAnswersWithComments(userId, t.Result);
                    }).Unwrap();
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 获取一组带有评论信息的回答。
        /// </summary>
        /// <param name="userId">提出请求的用户ID</param>
        /// <param name="ans">一组答案数据</param>
        /// <returns>一组加载了评论信息的答案信息</returns>
        public async Task<IEnumerable<Answer>> GetAnswersWithComments(string userId, IEnumerable<AVObject> ans)
        {
            try
            {
                if (ans.Count() > 0)
                {
                    if (ans.First().ClassName != "Answer")
                    {
                        throw new InvalidOperationException("获取的对象不是答案类object。");
                    }

                    List<Task<Answer>> tl = new List<Task<Answer>>();

                    foreach (AVObject a in ans)
                    {
                        tl.Add(GetAnswerWithComments(userId, a).ContinueWith(t =>
                        {
                            if (t.IsFaulted || t.IsCanceled)
                            {
                                throw t.Exception;
                            }

                            return t.Result;
                        }));
                    }

                    await Task.WhenAll(tl.ToArray());

                    List<Answer> answers = new List<Answer>();

                    foreach (Task<Answer> t in tl)
                    {
                        answers.Add(t.Result);
                    }

                    return answers;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 获取一条带有评论信息的回答。
        /// </summary>
        /// <param name="answerId">答案数据ID</param>
        /// <returns>加载了评论信息的答案信息</returns>
        public async Task<Answer> GetAnswerWithComments(string answerId)
        {
            AVObject a = AVObject.CreateWithoutData("Answer", answerId);

            Task<Answer> ta = AVObject.GetQuery("Answer")
                .Include("createdBy")
                .Include("forQuestion")
                .GetAsync(answerId)
                .ContinueWith(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                        {
                            throw t.Exception;
                        }

                        UpdateViewCount(t.Result);

                        return new Answer(t.Result);
                    });

            Task<IEnumerable<AVObject>> tc = AVObject.GetQuery("Comment")
                .Include("createdBy")
                .WhereEqualTo("forAnswer", AVObject.CreateWithoutData("Answer", answerId))
                .OrderByDescending("createdAt")
                .FindAsync()
                .ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    return t.Result;
                });

            await Task.WhenAll(ta, tc);

            return ta.Result.SetComments(tc.Result);
        }
        /// <summary>
        /// 获取一条带有评论信息的回答。
        /// </summary>
        /// <param name="userId">提出请求的用户ID</param>
        /// <param name="a">一条答案数据</param>
        /// <returns>加载了评论信息的答案信息</returns>
        public async Task<Answer> GetAnswerWithComments(string userId, AVObject a)
        {
            try
            {
                if (a.ClassName != "Answer")
                {
                    throw new InvalidOperationException("获取的对象不是答案类object。");
                }

                Task<IEnumerable<AVObject>> cmts = AVObject.GetQuery("Comment")
                    .Include("createdBy")
                    .WhereEqualTo("forAnswer", a)
                    .OrderByDescending("createdAt")
                    .FindAsync()
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                        {
                            throw t.Exception;
                        }

                        return t.Result;
                    });

                Task<bool?> userVote = IsAnswerLikedByUser(userId, a);

                await Task.WhenAll(cmts, userVote);

                return new Answer(a).SetComments(cmts.Result).SetVote(userVote.Result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 通过答案ID查找评论。
        /// </summary>
        /// <param name="answerId">答案ID</param>
        /// <returns>答案下所有评论</returns>
        public async Task<IEnumerable<Comment>> FindCommentsByAnswer(string answerId)
        {
            return await AVObject.GetQuery("Comment")
                .Include("createdBy")
                .WhereEqualTo("forAnswer", AVObject.CreateWithoutData("Answer", answerId))
                .OrderByDescending("createdAt")
                .FindAsync()
                .ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    List<Comment> cmts = new List<Comment>();

                    foreach (AVObject cmt in t.Result)
                    {
                        cmts.Add(new Comment(cmt));
                    }

                    return cmts;
                });
        }
        /// <summary>
        /// 查询特定用户对于一条问题的点赞状态。
        /// </summary>
        /// <param name="userId">特定用户的ID</param>
        /// <param name="question">指定的问题条目</param>
        /// <returns>点赞情况：“true”为赞，“false”为踩，空值为未表态</returns>
        public async Task<bool?> IsQuestionLikedByUser(string userId, AVObject question)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            return await AVObject.GetQuery("VotePost")
                .WhereEqualTo("voteBy", AVObject.CreateWithoutData("_User", userId) as AVUser)
                .WhereEqualTo("voteFor", question)
                .FindAsync()
                .ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    return t.Result.Count() > 0 ? t.Result.First().Get<bool?>("isLike") : null;
                });
        }
        /// <summary>
        /// 查询特定用户对于一条答案的点赞状态
        /// </summary>
        /// <param name="userId">特定用户的</param>
        /// <param name="answer">指定的答案条目</param>
        /// <returns>点赞情况：“true”为赞，“false”为踩，空值为未表态</returns>
        public async Task<bool?> IsAnswerLikedByUser(string userId, AVObject answer)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            return await AVObject.GetQuery("VoteAnswer")
                .WhereEqualTo("voteBy", AVObject.CreateWithoutData("_User", userId) as AVUser)
                .WhereEqualTo("voteFor", answer)
                .FindAsync()
                .ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    return t.Result.Count() > 0 ? t.Result.First().Get<bool?>("isLike") : null;
                });
        }
        /// <summary>
        /// 为问题或者答案条目更新浏览量。
        /// </summary>
        /// <param name="obj">目标问题或答案</param>
        /// <returns></returns>
        public async Task UpdateViewCount(AVObject obj)
        {
            obj["viewCount"] = obj.ContainsKey("viewCount") ? obj.Get<int>("viewCount") + 1 : 1;
            await obj.SaveAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }
                });
        }
        #endregion

        #region Voting
        /// <summary>
        /// 保存赞或踩。
        /// </summary>
        /// <param name="v">动作信息</param>
        /// <returns>代表操作是否成功的Boolean值</returns>
        public async Task<bool> PerformVote(Vote v)
        {
            if (v.IsUpdate)
            {
                return await AVObject.GetQuery(v.LoadClassName()).WhereEqualTo("voteFor", v.LoadTargetObject()).WhereEqualTo("voteBy", v.LoadVoter()).FirstAsync().ContinueWith(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                        {
                            throw t.Exception;
                        }

                        t.Result["isLike"] = v.IsLike;
                        return t.Result.SaveAsync().ContinueWith(s =>
                            {
                                if (s.IsFaulted || s.IsCanceled)
                                {
                                    throw s.Exception;
                                }

                                return UpdateVoteDiff(v);
                            });
                    }).Unwrap().Unwrap();
            }
            else
            {
                return await v.CreateVoteObject().SaveAsync().ContinueWith(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                        {
                            throw t.Exception;
                        }

                        return UpdateVoteDiff(v);
                    }).Unwrap();
            }
        }
        /// <summary>
        /// 更新点赞对象数据中的差票值。
        /// </summary>
        /// <param name="v">动作信息</param>
        /// <returns>代表操作是否成功的Boolean值</returns>
        public async Task<bool> UpdateVoteDiff(Vote v)
        {
            AVObject target = v.LoadTargetObject();

            return await target.FetchAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    int valueChange = v.IsUpdate ? 2 : 1;
                    target["voteDiff"] = v.IsLike ? target.Get<int>("voteDiff") + valueChange : target.Get<int>("voteDiff") - valueChange;
                    return target.SaveAsync().ContinueWith(s =>
                    {
                        if (s.IsFaulted || s.IsCanceled)
                        {
                            throw s.Exception;
                        }

                        return true;
                    });
                }).Unwrap();
        }
        #endregion

        #region Post Save Operation
        /// <summary>
        /// 保存一个提问到数据库。
        /// </summary>
        /// <param name="qsn">问题内容对象</param>
        /// <returns>代表保存是否成功的Boolean值</returns>
        public async Task<bool> SaveNewQuestion(Question qsn)
        {
            AVObject q = qsn.CreateQuestionObjectForSave();

            return await q.SaveAsync().ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }
                else
                {
                    return true;
                }
            });
        }
        /// <summary>
        /// 保存一个回答到数据库。
        /// </summary>
        /// <param name="ans">答案内容对象</param>
        /// <returns>代表保存是否成功的Boolean值</returns>
        public async Task<bool> SaveNewAnswer(Answer ans)
        {
            AVObject a = ans.CreateAnswerObjectForSave();

            return await a.SaveAsync().ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }

                return a.Get<AVObject>("forQuestion").FetchAsync().ContinueWith(s =>
                    {
                        if (s.IsFaulted || s.IsCanceled)
                        {
                            throw s.Exception;
                        }

                        UpdateSubpostCount(s.Result, true).ContinueWith(x =>
                        {
                            if (x.IsFaulted || x.IsCanceled)
                            {
                                throw x.Exception;
                            }
                        });

                        return true;
                    });
            }).Unwrap();
        }
        /// <summary>
        /// 保存一个评论到数据库。
        /// </summary>
        /// <param name="cmt">评论内容对象</param>
        /// <returns>代表保存是否成功的Boolean值</returns>
        public async Task<bool> SaveNewComment(Comment cmt)
        {
            AVObject c = cmt.CreateCommentObjectForSave();

            return await c.SaveAsync().ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }

                return c.Get<AVObject>("forAnswer").FetchAsync().ContinueWith(s =>
                    {
                        if (s.IsFaulted || s.IsCanceled)
                        {
                            throw s.Exception;
                        }

                        UpdateSubpostCount(s.Result, true).ContinueWith(x =>
                            {
                                if (x.IsFaulted || s.IsCanceled)
                                {
                                    throw x.Exception;
                                }
                            });

                        return true;
                    });
            }).Unwrap();
        }
        /// <summary>
        /// 保存用户对自己发过的问题或答案内容的更新。
        /// </summary>
        /// <param name="isQuestion">被更新目标是否为问题</param>
        /// <param name="id">被更新目标的ID</param>
        /// <param name="content">更新后的内容</param>
        /// <returns>代表操作是否成功的Boolean值</returns>
        public async Task<bool> UpdateContent(bool isQuestion, string id, string content)
        {
            string className = isQuestion ? "Post" : "Answer";

            return await AVObject.GetQuery(className).GetAsync(id).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    t.Result["content"] = content;
                    return t.Result.SaveAsync().ContinueWith(s =>
                    {
                        if (s.IsFaulted || s.IsCanceled)
                        {
                            throw s.Exception;
                        }

                        return true;
                    });
                }).Unwrap();
        }
        /// <summary>
        /// 删除一条答案及其包含的评论。
        /// </summary>
        /// <param name="ansId">需要删除的答案ID</param>
        /// <returns>代表操作是否成功的Boolean值</returns>
        public async Task<bool> DeleteAnswerWithComments(string ansId)
        {
            //AVObject ans = AVObject.CreateWithoutData("Answer", ansId);
            List<Task> ts = new List<Task>();

            AVObject ans = await AVObject.GetQuery("Answer").Include("forQuestion").GetAsync(ansId);
            IEnumerable<AVObject> cmts = await AVObject.GetQuery("Comment").WhereEqualTo("forAnswer", ans).FindAsync();

            foreach (AVObject cmt in cmts)
            {
                ts.Add(cmt.DeleteAsync().ContinueWith(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                        {
                            throw t.Exception;
                        }
                    }));
            }

            ts.Add(ans.DeleteAsync().ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }

                UpdateSubpostCount(ans.Get<AVObject>("forQuestion"), false).ContinueWith(s =>
                    {
                        if (s.IsFaulted || s.IsCanceled)
                        {
                            throw s.Exception;
                        }
                    });
            }));

            Task.WaitAll(ts.ToArray());

            return true;
        }
        /// <summary>
        /// 删除一条评论。
        /// </summary>
        /// <param name="cmtId">需要删除的评论ID</param>
        /// <returns>代表操作是否成功的Boolean值</returns>
        public async Task<bool> DeleteComment(string cmtId)
        {
            return await AVObject.GetQuery("Comment").Include("forAnswer").GetAsync(cmtId).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    UpdateSubpostCount(t.Result.Get<AVObject>("forAnswer"), false).ContinueWith(s =>
                        {
                            if (s.IsFaulted || s.IsCanceled)
                            {
                                throw s.Exception;
                            }
                        });

                    return t.Result.DeleteAsync().ContinueWith(s =>
                        {
                            if (s.IsFaulted || s.IsCanceled)
                            {
                                throw s.Exception;
                            }
                            return true;
                        });
                }).Unwrap();

            //return await AVObject.CreateWithoutData("Comment", cmtId).DeleteAsync().ContinueWith(t =>
            //    {
            //        if (t.IsFaulted || t.IsCanceled)
            //        {
            //            throw t.Exception;
            //        }

            //        return true;
            //    });
        }
        /// <summary>
        /// 为问题或答案更新回复数量。
        /// </summary>
        /// <param name="obj">目标问题或答案</param>
        /// <param name="isIncrement">是否为增加操作</param>
        /// <returns></returns>
        public async Task UpdateSubpostCount(AVObject obj, bool isIncrement)
        {
            if (!obj.ContainsKey("subPostCount") && !isIncrement)
            {
                throw new InvalidOperationException("没有可操作的次级内容！");
            }

            obj["subPostCount"] = isIncrement ? (obj.ContainsKey("subPostCount") ? obj.Get<int>("subPostCount") + 1 : 1) : (obj.Get<int>("subPostCount") - 1);
            await obj.SaveAsync().ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }
            });
        }
        #endregion

        #region Message Operation
        /// <summary>
        /// 查找用户收到的所有消息。
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>一组消息</returns>
        public async Task<IEnumerable<Message>> LoadMessagesForUser(string userId)
        {
            #region CQL Code
            //string cqlString = string.Format("select include content, include from, include to, * from Message where to in (pointer('_User','{0}') , pointer('_User',null))", userId);

            //return await AVQuery<AVObject>.DoCloudQuery(cqlString).ContinueWith(t =>
            //{
            //    if (t.IsFaulted || t.IsCanceled)
            //    {
            //        throw t.Exception;
            //    }
            //    else
            //    {
            //        List<Message> messages = new List<Message>();

            //        foreach (AVObject message in t.Result.Where(x => !x.Get<bool>("isDeleted")).GroupBy(x => x.Get<AVObject>("content").ObjectId).Select(x => x.Count() > 1 ? x.Where(y => y.ContainsKey("to")).First() : x.First()).OrderByDescending(x => x.Get<AVObject>("content").CreatedAt))
            //        {
            //            messages.Add(new Message(message));
            //        }

            //        return messages;
            //    }
            //});
            #endregion

            #region AVQuery Code
            List<AVQuery<AVObject>> querys = new List<AVQuery<AVObject>>()
            {
                AVObject.GetQuery("Message").WhereEqualTo("to", null),
                AVObject.GetQuery("Message").WhereEqualTo("to", AVObject.CreateWithoutData("_User", userId) as AVUser)
            };

            IEnumerable<AVObject> msgs = await AVQuery<AVObject>.Or(querys).Include("from").Include("to").Include("content").FindAsync().ContinueWith(t =>  //.WhereEqualTo("isNew", true).WhereEqualTo("isDeleted", false)
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }
                    else
                    {
                        return t.Result.GroupBy(x => x.Get<AVObject>("content").ObjectId).Select(x => x.Count() > 1 ? x.Where(y => y.ContainsKey("to")).First() : x.First()).Where(x => !x.Get<bool>("isDeleted")).OrderByDescending(x => x.Get<bool>("isNew"));
                    }
                });

            List<Message> messages = new List<Message>();

            foreach (AVObject message in msgs)
            {
                messages.Add(new Message(message));
            }

            return messages;
            #endregion
        }
        /// <summary>
        /// 读取一条消息。
        /// </summary>
        /// <param name="messageId">消息ID</param>
        /// <returns>消息对象</returns>
        public async Task<Message> GetMessageByID(string messageId)
        {
            return await AVObject.GetQuery("Message").Include("from").Include("to").Include("content").GetAsync(messageId).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    return new Message(t.Result);
                });
        }
        /// <summary>
        /// 查询用户的新消息数量。
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>新消息的数量</returns>
        public async Task<int> CountNewMessageForUser(string userId)
        {
            List<AVQuery<AVObject>> querys = new List<AVQuery<AVObject>>()
            {
                AVObject.GetQuery("Message").WhereEqualTo("to", null),
                AVObject.GetQuery("Message").WhereEqualTo("to", AVObject.CreateWithoutData("_User", userId) as AVUser)
            };

            return await AVQuery<AVObject>.Or(querys).Include("to").FindAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }
                    else
                    {
                        return t.Result.GroupBy(x => x.Get<AVObject>("content").ObjectId).Select(x => x.Count() > 1 ? x.Where(y => y.ContainsKey("to")).First() : x.First()).Where(x => x.Get<bool>("isNew") && !x.Get<bool>("isDeleted")).Count();
                    }
                });
        }
        /// <summary>
        /// 创建一条消息。
        /// </summary>
        /// <param name="msg">消息内容对象</param>
        /// <returns>代表创建是否成功的Boolean值</returns>
        public async Task<bool> WriteNewMessage(Message msg)
        {
            AVObject message = msg.CreateMessageObjectForWrite();

            return await message.SaveAsync().ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }
                else
                {
                    return true;
                }
            });
        }
        /// <summary>
        /// 把一条消息标记为已读。
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="messageId">消息ID</param>
        /// <returns></returns>
        public async Task<bool> MarkMessageAsOpened(string userId, string messageId)
        {
            AVObject message = await AVObject.GetQuery("Message").Include("to").Include("from").Include("content").GetAsync(messageId);

            if (!message.Get<bool>("isNew"))
            {
                return true;
            }

            if (message.Get<AVUser>("to") == null)
            {
                AVUser u = AVUser.CreateWithoutData("_User", userId) as AVUser;
                AVObject newMsg = new AVObject("Message");

                newMsg.Add("from", message.Get<AVUser>("from"));
                newMsg.Add("content", message.Get<AVObject>("content"));
                newMsg.Add("to", u);
                newMsg.Add("isNew", false);

                return await newMsg.SaveAsync().ContinueWith(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                        {
                            throw t.Exception;
                        }

                        return true;
                    });
            }

            message["isNew"] = false;

            return await message.SaveAsync().ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }

                return true;
            });
        }
        /// <summary>
        /// 删除一封消息。
        /// </summary>
        /// <param name="userId">消息接收人ID</param>
        /// <param name="messageId">消息ID</param>
        /// <returns>代表删除是否成功的Boolean值</returns>
        public async Task<bool> DeleteMessage(string userId, string messageId)
        {
            AVObject message = await AVObject.GetQuery("Message").Include("content").Include("to").GetAsync(messageId);

            if (message.Get<AVUser>("to").ObjectId == userId)
            {
                bool isSuccess = true;

                if (!Convert.ToBoolean(message.Get<AVObject>("content")["isSystem"]))
                {
                    Task t1 = message.Get<AVObject>("content").DeleteAsync().ContinueWith(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled || !isSuccess)
                        {
                            isSuccess = false;
                            throw t.Exception;
                        }
                    });

                    Task t2 = message.DeleteAsync().ContinueWith(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled || !isSuccess)
                        {
                            isSuccess = false;
                            throw t.Exception;
                        }
                    });

                    Task.WaitAll(t1, t2);
                }
                else
                {
                    message["isDeleted"] = true;

                    await message.SaveAsync().ContinueWith(t =>
                        {
                            if (t.IsFaulted || t.IsCanceled)
                            {
                                isSuccess = false;
                                throw t.Exception;
                            }
                        });
                }

                return isSuccess;
            }

            throw new InvalidOperationException("不能删除系统邮件内容");
        }
        #endregion

        #region File Operation
        /// <summary>
        /// 将一个二进制文件数据存入数据库，并返回Url。
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="fileContent">文件的二进制数据</param>
        /// <param name="metaData">字典结构的文件附带信息</param>
        /// <returns>文件存入后生成的Url</returns>
        public async Task<string> SaveNewByteFile(RTCareerAsk.DAL.Domain.File f)
        {
            AVFile file = f.CreateStreamFileObjectForSave();

            return await file.SaveAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    return file.Url.OriginalString;
                });
        }
        /// <summary>
        /// 将一个数据流文件数据存入数据库，并返回Url。
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="fileContent">文件的数据流数据</param>
        /// <param name="metaData">字典结构的文件附带信息</param>
        /// <returns>文件存入后生成的Url</returns>
        public async Task<string> SaveNewStreamFile(RTCareerAsk.DAL.Domain.File f)
        {
            AVFile file = f.CreateStreamFileObjectForSave();

            return await file.SaveAsync().ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }

                return file.Url.OriginalString;
            });
        }
        /// <summary>
        /// 找出数据库存有的所有文件。
        /// </summary>
        /// <returns>文件显示数据的集合</returns>
        public async Task<IEnumerable<Domain.FileInfo>> FindAllFiles()
        {
            try
            {
                return await AVObject.GetQuery("_File").FindAsync().ContinueWith(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                        {
                            throw t.Exception;
                        }

                        return GenerateFileInfoObjects(t.Result);
                    });
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 找出数据库属性符合期待值文件
        /// </summary>
        /// <param name="propKey">属性键名</param>
        /// <param name="propValue">属性值</param>
        /// <returns>符合要求的文件显示数据的集合</returns>
        public async Task<IEnumerable<Domain.FileInfo>> FindFilesByProperty(string propKey, object propValue)
        {
            try
            {
                return await AVObject.GetQuery("_File").WhereEqualTo(propKey, propValue).FindAsync().ContinueWith(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                        {
                            throw t.Exception;
                        }

                        return GenerateFileInfoObjects(t.Result);
                    });
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 根据文件ID下载文件数据。
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <returns>文件二进制数据</returns>
        public async Task<Domain.File> DownloadFileByID(string fileId)
        {
            AVFile file = await AVFile.GetFileWithObjectIdAsync(fileId);

            await file.DownloadAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }
                });

            return new Domain.File(file);
        }
        /// <summary>
        /// 通过文件的URL删除文件。
        /// </summary>
        /// <param name="url">文件的URL地址</param>
        /// <returns>代表操作是否成功的Boolean值</returns>
        public async Task<bool> DeleteFileWithUrl(string url)
        {
            return await AVObject.GetQuery("_File").WhereEqualTo("url", url).FirstAsync().ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }

                return t.Result.DeleteAsync().ContinueWith(s =>
                    {
                        if (s.IsFaulted || s.IsCanceled)
                        {
                            throw s.Exception;
                        }

                        return true;
                    }); ;
            }).Unwrap();
        }
        #endregion

        #region Bug Report
        /// <summary>
        /// 读取所有的错误报告
        /// </summary>
        /// <returns>按照优先级排序的错误报告</returns>
        public async Task<List<Bug>> LoadBugReports()
        {
            List<Bug> bugs = new List<Bug>();

            return await AVObject.GetQuery("Bug").Include("reporter").FindAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    foreach (AVObject bug in t.Result.OrderBy(x => x.Get<int>("status")))
                    {
                        bugs.Add(new Bug(bug));
                    }

                    return bugs;
                });
        }
        /// <summary>
        /// 保存一个报告到数据库。
        /// </summary>
        /// <param name="b">错误报告</param>
        /// <returns>代表保存是否成功的Boolean值</returns>
        public async Task<bool> SaveNewBugReport(Bug b)
        {
            b.BugIndex = await AVObject.GetQuery("Bug").OrderByDescending("bugIndex").FirstAsync().ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }

                return t.Result.Get<int>("bugIndex") + 1;
            });

            if (b.AttachmentFile != null)
            {
                b.Attachment = await SaveNewStreamFile(b.AttachmentFile);
            }

            return await b.CreateBugObjectForSave().SaveAsync().ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }

                return true;
            });
        }
        /// <summary>
        /// 更新一条报告。
        /// </summary>
        /// <param name="b">错误报告</param>
        /// <returns>代表保存是否成功的Boolean值</returns>
        public async Task<bool> UpdateBugReport(Bug b)
        {
            #region Code for Debug
            //AVObject bug = await AVObject.GetQuery("Bug").Include("reporter").GetAsync(b.ObjectID);

            //bug["priority"] = b.Priority;
            //bug["status"] = b.StatusCode;

            //await bug.SaveAsync().ContinueWith(t =>
            //    {
            //        if (t.IsFaulted || t.IsCanceled)
            //        {
            //            throw t.Exception;
            //        }
            //    });

            //return new Bug(bug);
            #endregion

            return await AVObject.GetQuery("Bug").GetAsync(b.ObjectID).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    t.Result["priority"] = b.Priority;
                    t.Result["status"] = b.StatusCode;

                    t.Result.SaveAsync().ContinueWith(s =>
                        {
                            if (s.IsFaulted || s.IsCanceled)
                            {
                                throw s.Exception;
                            }
                        });

                    return true;
                });
        }
        #endregion

        #endregion

        #region Tests

        public async Task<bool> CreateNewTable(string tableName, IDictionary<string, object> properties)
        {
            bool isSuccess = false;

            AVObject newTable = AVObject.Create(tableName);

            foreach (KeyValuePair<string, object> property in properties)
            {
                newTable.Add(property.Key, property.Value);
            }

            await newTable.SaveAsync().ContinueWith(t =>
                {
                    if (!t.IsCanceled && !t.IsFaulted)
                    {
                        isSuccess = true;
                    }
                });

            return isSuccess;
        }

        public async Task<bool> CreateTestAnswer(string content, string userId)
        {
            bool isSuccess = false;

            AVObject ansTst = new AVObject("AnswerTest");
            ansTst.Add("content", content);
            ansTst.Add("createdBy", await AVUser.Query.GetAsync(userId));

            await ansTst.SaveAsync().ContinueWith(t =>
                {
                    if (!t.IsCanceled && !t.IsFaulted)
                    {
                        isSuccess = true;
                    }
                });

            return isSuccess;
        }

        public async Task<bool> CreateTestComment(string content, string userId)
        {
            bool isSuccess = false;

            AVObject cmtTst = new AVObject("CommentTest");
            cmtTst.Add("content", content);
            cmtTst.Add("createdBy", await AVUser.Query.GetAsync(userId));//AVObject.GetQuery("_User").GetAsync(userId)

            await cmtTst.SaveAsync().ContinueWith(t =>
            {
                if (!t.IsCanceled && !t.IsFaulted)
                {
                    isSuccess = true;
                }
            });

            return isSuccess;
        }

        public async Task<bool> UpdateAnswerContent(string answerId, string content)
        {
            return await AVObject.GetQuery("Answer").GetAsync(answerId).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    t.Result["content"] = content;
                    return t.Result.SaveAsync().ContinueWith(s =>
                    {
                        if (s.IsFaulted || s.IsCanceled)
                        {
                            throw s.Exception;
                        }

                        return true;
                    });
                }).Unwrap();
        }

        public async Task<AVUser> GetUserByID(string userId)
        {
            return await AVUser.Query.GetAsync(userId);
        }

        public async Task<AVObject> GetMessageBodyByID(string msgBodyId)
        {
            return await AVObject.GetQuery("Message_Body").GetAsync(msgBodyId);
        }

        public async Task<byte[]> DownloadFile()
        {
            AVFile file = await AVFile.GetFileWithObjectIdAsync("578bdd8a165abd0067250a58");
            await file.DownloadAsync();
            return file.DataByte;
            //.ContinueWith(t =>
            //{
            //    var file = t.Result;
            //    file.DownloadAsync().ContinueWith(s =>
            //    {
            //        var dataByte = file.DataByte;//获取文件流的byte数组，之后可以做保存，发送等操作。
            //    });
            //});
        }

        public async Task RelationSetting()
        {
            Task<AVUser> u1 = AVUser.Query.GetAsync("578a84278ac24700608b9f21");
            Task<AVUser> u2 = AVUser.Query.GetAsync("578101cad342d30057c928a5");
            Task<AVObject> test = AVObject.GetQuery("Test").GetAsync("578d1ff72e958a00543dc8db");

            Task.WaitAll(u1, u2);

            AVRelation<AVUser> relation = test.Result.GetRelation<AVUser>("forUser");

            relation.Add(u1.Result);
            relation.Add(u2.Result);

            await test.Result.SaveAsync();
        }

        public async Task<IEnumerable<AVObject>> SearchByRelation()
        {
            AVUser u = AVUser.CreateWithoutData("_User", "578101cad342d30057c928a5") as AVUser;
            AVQuery<AVObject> query = AVObject.GetQuery("Test").WhereEqualTo("forUser", u);

            return await query.FindAsync();
        }

        public async Task UpdateRelation()
        {
            Task<AVUser> u1 = AVUser.Query.GetAsync("578a84278ac24700608b9f21");
            Task<AVObject> test = AVObject.GetQuery("Test").Include("forUser").GetAsync("578d1ff72e958a00543dc8db").ContinueWith(t =>
                {
                    t.Result.Get<AVRelation<AVUser>>("forUser").Add(u1.Result);
                    return t.Result;
                });

            Task.WaitAll(u1, test);

            await test.Result.SaveAsync();
        }

        public async Task DeleteRelation()
        {
            AVObject test = await AVObject.GetQuery("Test").Include("forUser").GetAsync("578d1ff72e958a00543dc8db");

            test.Remove("forUser");

            await test.SaveAsync().ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        throw t.Exception;
                    }
                });
        }

        public async Task UpdateFile(string fileId)
        {
            AVFile file = await AVFile.GetFileWithObjectIdAsync(fileId);

            await file.SaveAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }
                });
        }

        public async Task<AVObject> FindFileByUrl(string url)
        {
            return await AVObject.GetQuery("_File").WhereEqualTo("url", url).FirstAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    return t.Result;
                });
        }

        public async Task<IEnumerable<AVObject>> GetAnswer100TimesAsync(string ansId)
        {
            List<Task<AVObject>> ansTasks = new List<Task<AVObject>>();
            List<AVObject> ansResults = new List<AVObject>();

            for (int i = 0; i < 20; i++)
            {
                ansTasks.Add(AVObject.GetQuery("Answer").GetAsync(ansId));
            }

            await Task.WhenAll(ansTasks.ToArray());

            foreach (Task<AVObject> t in ansTasks)
            {
                ansResults.Add(t.Result);
            }

            return ansResults;
        }

        public async Task<IEnumerable<AVObject>> GetAnswer100Times(string ansId)
        {
            List<AVObject> ansResults = new List<AVObject>();

            for (int i = 0; i < 20; i++)
            {
                ansResults.Add(await AVObject.GetQuery("Answer").GetAsync(ansId));
            }

            return ansResults;
        }

        public async Task UpdateSubpostCountForQuestion(AVObject question)
        {
            if (question.ClassName != "Post")
            {
                throw new InvalidOperationException("对象类型不是问题类别，实际类型：" + question.ClassName);
            }

            await AVObject.GetQuery("Answer").WhereEqualTo("forQuestion", question).CountAsync().ContinueWith(async t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    throw t.Exception;
                }

                question["subPostCount"] = t.Result;
                await question.SaveAsync().ContinueWith(s =>
                    {
                        if (s.IsFaulted || s.IsCanceled)
                        {
                            throw s.Exception;
                        }
                    });
            });
        }

        public async Task UpdateSubpostCountForAnswer(AVObject answer)
        {
            if (answer.ClassName != "Answer")
            {
                throw new InvalidOperationException("对象类型不是答案类别，实际类型：" + answer.ClassName);
            }

            await AVObject.GetQuery("Comment").WhereEqualTo("forAnswer", answer).CountAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw t.Exception;
                    }

                    answer["subPostCount"] = t.Result;
                    answer.SaveAsync().ContinueWith(s =>
                        {
                            if (s.IsFaulted || s.IsCanceled)
                            {
                                throw s.Exception;
                            }
                        });
                });
        }

        public async Task<bool> UpdateSubpostCountForQuestions()
        {
            List<Task> updateTask = new List<Task>();

            IEnumerable<AVObject> questions = await AVObject.GetQuery("Post").FindAsync();

            foreach (AVObject qsn in questions)
            {
                updateTask.Add(UpdateSubpostCountForQuestion(qsn));
            }

            await Task.WhenAll(updateTask.ToArray());

            return true;
        }

        public async Task<bool> UpdateSubpostCountForAnswers()
        {
            List<Task> updateTask = new List<Task>();

            IEnumerable<AVObject> answers = await AVObject.GetQuery("Answer").FindAsync();

            foreach (AVObject ans in answers)
            {
                updateTask.Add(UpdateSubpostCountForAnswer(ans));
            }

            await Task.WhenAll(updateTask.ToArray());

            return true;
        }

        #endregion

        #region Helper

        private IEnumerable<Domain.FileInfo> GenerateFileInfoObjects(IEnumerable<AVObject> fs)
        {
            if (fs.Count() == 0 || fs.First().ClassName != "_File")
            {
                throw new InvalidOperationException("无法获取文件。");
            }

            List<Domain.FileInfo> fis = new List<Domain.FileInfo>();

            foreach (AVObject f in fs)
            {
                fis.Add(GenereateFileInfoObject(f));
            }

            return fis;
        }

        private Domain.FileInfo GenereateFileInfoObject(AVObject f)
        {
            if (f.ClassName != "_File")
            {
                throw new InvalidOperationException("获取的对象不是文件类object。");
            }

            #region Code for Debug
            //Domain.FileInfo fi = new Domain.FileInfo();

            //fi.ObjectID = f.ObjectId;
            //fi.Mime_Type = f.Get<string>("mime_type");
            //fi.Key = f.Get<string>("key");
            //fi.FileName = f.Get<string>("name");
            //fi.Url = f.Get<string>("url");
            //fi.MetaData = f.Get<IDictionary<string,object>>("metaData");
            //fi.DateCreate = Convert.ToDateTime(f.CreatedAt);

            //return fi;
            #endregion

            return new Domain.FileInfo()
            {
                ObjectID = f.ObjectId,
                Mime_Type = f.Get<string>("mime_type"),
                Key = f.Get<string>("key"),
                FileName = f.Get<string>("name"),
                Url = f.Get<string>("url"),
                MetaData = f.Get<IDictionary<string, object>>("metaData"),
                DateCreate = Convert.ToDateTime(f.CreatedAt)
            };
        }

        #endregion

        #region Trunk

        /// <summary>
        /// 查找所有的提问。
        /// </summary>
        /// <returns>所有已存在的问题队列</returns>
        //public async Task<IEnumerable<QuestionInfo>> FindQuestionList()
        //{
        //    try
        //    {
        //        return await AVObject.GetQuery("Post").Include("createdBy").FindAsync().ContinueWith(t =>
        //            {
        //                if (t.IsFaulted || t.IsCanceled)
        //                {
        //                    throw t.Exception;
        //                }

        //                return GetQuestionsWithAnswerCounts(t.Result).ContinueWith(s =>
        //                    {
        //                        return s.Result.OrderByDescending(x => x.AnswerCount);
        //                    });
        //            }).Unwrap();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        /// <summary>
        /// 生成一组带有答案数量的问题概述。
        /// </summary>
        /// <param name="qs">一组问题</param>
        /// <returns>一组带有答案数量的问题概述</returns>
        //public async Task<IEnumerable<QuestionInfo>> GetQuestionsWithAnswerCounts(IEnumerable<AVObject> qs)
        //{
        //    try
        //    {
        //        if (qs.Count() > 0)
        //        {
        //            if (qs.First().ClassName != "Post")
        //            {
        //                throw new InvalidOperationException(string.Format("获取的对象{0}不是问题类object。对象类型：{1}", qs.First().ObjectId, qs.First().ClassName));
        //            }

        //            List<Task<QuestionInfo>> tl = new List<Task<QuestionInfo>>();

        //            foreach (AVObject q in qs)
        //            {
        //                tl.Add(GetQuestionWithAnswerCount(q));
        //            }

        //            await Task.WhenAll(tl.ToArray());

        //            List<QuestionInfo> qis = new List<QuestionInfo>();

        //            foreach (Task<QuestionInfo> t in tl)
        //            {
        //                qis.Add(t.Result);
        //            }

        //            return qis;
        //        }

        //        return null;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        /// <summary>
        /// 生成一个带有答案数量的问题概述。
        /// </summary>
        /// <param name="q">问题</param>
        /// <returns>带有答案数量的问题概述</returns>
        //public async Task<QuestionInfo> GetQuestionWithAnswerCount(AVObject q)
        //{
        //    try
        //    {
        //        if (q.ClassName != "Post")
        //        {
        //            throw new InvalidOperationException("获取的对象不是问题类object。");
        //        }

        //        return await AVObject.GetQuery("Answer").WhereEqualTo("forQuestion", q).CountAsync().ContinueWith(t =>
        //            {
        //                if (t.IsFaulted || t.IsCanceled)
        //                {
        //                    throw t.Exception;
        //                }

        //                return new QuestionInfo(q, t.Result);
        //            });
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        /// <summary>
        /// 查找所有的答案。
        /// </summary>
        /// <returns>所有已存在的答案队列</returns>
        //public async Task<IEnumerable<AnswerInfo>> FindAnswerList()
        //{
        //    try
        //    {
        //        return await AVObject.GetQuery("Answer").Include("forQuestion").Include("createdBy").FindAsync().ContinueWith(t =>
        //            {
        //                if (t.IsFaulted || t.IsCanceled)
        //                {
        //                    throw t.Exception;
        //                }

        //                return GetAnswersWithCommentCounts(t.Result).ContinueWith(s =>
        //                    {
        //                        return s.Result.OrderByDescending(x => x.CommentCount);
        //                    });
        //            }).Unwrap();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        /// <summary>
        /// 生成一组带有评论数量的答案概述。
        /// </summary>
        /// <param name="ans">一组答案</param>
        /// <returns>一组带有评论数量的答案概述</returns>
        //public async Task<IEnumerable<AnswerInfo>> GetAnswersWithCommentCounts(IEnumerable<AVObject> ans)
        //{
        //    try
        //    {
        //        if (ans.Count() > 0)
        //        {
        //            if (ans.First().ClassName != "Answer")
        //            {
        //                throw new InvalidOperationException(string.Format("获取的对象{0}不是答案类object。对象类型：{1}", ans.First().ObjectId, ans.First().ClassName));
        //            }

        //            List<Task<AnswerInfo>> tl = new List<Task<AnswerInfo>>();

        //            foreach (AVObject a in ans)
        //            {
        //                tl.Add(GetAnswerWithCommentCount(a));
        //            }

        //            await Task.WhenAll(tl.ToArray());

        //            List<AnswerInfo> ais = new List<AnswerInfo>();

        //            foreach (Task<AnswerInfo> t in tl)
        //            {
        //                ais.Add(t.Result);
        //            }

        //            return ais;
        //        }

        //        return null;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        /// <summary>
        /// 生成一个带有评论数量的答案概述。
        /// </summary>
        /// <param name="a">答案</param>
        /// <returns>带有评论数量的答案概述</returns>
        //public async Task<AnswerInfo> GetAnswerWithCommentCount(AVObject a)
        //{
        //    try
        //    {
        //        if (a.ClassName != "Answer")
        //        {
        //            throw new InvalidOperationException(string.Format("获取的对象{0}不是答案类object。对象类型：{1}", a.ObjectId, a.ClassName));
        //        }

        //        return await AVObject.GetQuery("Comment").WhereEqualTo("forAnswer", a).CountAsync().ContinueWith(t =>
        //            {
        //                if (t.IsFaulted || t.IsCanceled)
        //                {
        //                    throw t.Exception;
        //                }

        //                return new AnswerInfo(a, t.Result);
        //            });
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        #endregion
    }
}