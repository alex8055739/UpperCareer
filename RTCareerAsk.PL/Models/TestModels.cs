using System.ComponentModel.DataAnnotations;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.PL.Models
{
    public class BugModel
    {
        public BugModel() { }

        public BugModel(Bug bo)
        {
            CovertBugObjectToModel(bo);
        }

        public string BugID { get; set; }

        public int BugIndex { get; set; }

        public UserModel Reporter { get; set; }

        public int Priority { get; set; }

        public int StatusCode { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string AttachmentUrl { get; set; }

        private void CovertBugObjectToModel(Bug bo)
        {
            BugID = bo.ObjectID;
            BugIndex = bo.BugIndex;
            Reporter = bo.Reporter != null ? new UserModel(bo.Reporter) : null;
            Priority = bo.Priority;
            StatusCode = bo.StatusCode;
            Title = bo.Title;
            Description = bo.Description;
            AttachmentUrl = bo.Attachment;
        }

        public Bug CreateBugUpdateModel()
        {
            return new Bug()
            {
                ObjectID = BugID,
                Priority = Priority,
                StatusCode = StatusCode
            };
        }
    }

    public class CatchModel
    {
        public string ReporterID { get; set; }

        [Required(ErrorMessage = "请输入报告标题")]
        [Display(Name = "报告标题")]
        [StringLength(15, ErrorMessage = "标题请不要超过15个字")]
        public string Title { get; set; }

        [Required(ErrorMessage = "请输入问题描述")]
        [Display(Name = "问题描述")]
        public string Description { get; set; }

        [Required(ErrorMessage = "请选择优先级")]
        [Display(Name = "优先级")]
        public int Priority { get; set; }

        [Display(Name = "截图")]
        public FileModel Attachment { get; set; }

        public Bug CreateReportForSave()
        {
            return new Bug()
            {
                Reporter = new User() { ObjectID = ReporterID },
                Priority = Priority,
                StatusCode = 0,
                Title = Title,
                Description = Description,
                AttachmentFile = Attachment != null ? Attachment.RestoreFileModelToObject() : null
            };
        }
    }
}