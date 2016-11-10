var previewTarget = '#divCoverPreview',
    dropTarget = $(previewTarget);


function CoverPicSelected(e) {
    var files = e.target.files;

    PreviewPic(files, {
        previewTarget: previewTarget,
        postAction: function () {
            $('#btnUploadCover').removeClass('disabled');
        }
    })
}

function CoverPicDropped(e) {
    e.stopPropagation();
    e.preventDefault();
    dropTarget.removeClass('file-hover').removeClass('file-await');
    var files = e.originalEvent.dataTransfer.files;

    PreviewPic(files, {
        previewTarget: previewTarget,
        postAction: function () {
            $('#btnUploadCover').removeClass('disabled');
        }
    })
}

function DragOver(e) {
    e.stopPropagation();
    e.preventDefault();
    dropTarget.addClass('file-hover');
}

function DragLeave(e) {
    e.stopPropagation();
    e.preventDefault();
    dropTarget.removeClass('file-hover');
}

function FileDropped(e) {
    e.stopPropagation();
    e.preventDefault();
    $(e.target).parent().removeClass('file-hover').removeClass('file-await');

    if (window.File && window.FileReader && window.FileList && window.Blob) {
        var files = e.originalEvent.dataTransfer.files;

        for (var i = 0; i < files.length; i++) {
            if (!files[i].type.match('image.*')) {
                continue;
            }

            reader = new FileReader();
            reader.onload = (function (tFile) {
                return function (e) {
                    $('#divDropTarget').html('<img id="imgPortraitPreview" style="width: 100% ;" src="' + e.target.result + '" />');
                    ResizePortrait();
                    CropPortrait();
                };
            }(files[i]));
            reader.readAsDataURL(files[i]);
        }
    }
    else {
        alert('此浏览器不支持文件预览');
    }
}

$(document).ready(function () {
    dropTarget.on('dragover', DragOver);
    dropTarget.on('dragleave', DragLeave);
    dropTarget.on('drop', CoverPicDropped);

    $('#lnkUploadCover').click(function (e) {
        e.preventDefault();
        $('#inpCoverFile').trigger('click');
    })

    $('#inpCoverFile').on('change', CoverPicSelected);
})
