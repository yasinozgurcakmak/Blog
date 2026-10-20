document.addEventListener('DOMContentLoaded', function () {
    Fancybox.bind("[data-fancybox]", {});


});

var fbDialogAction = null;

function openFbDialog(message, confirmText, isDanger, onConfirm) {
    document.getElementById('fbDialogMessage').textContent = message;
    var confirmBtn = document.getElementById('fbDialogConfirm');
    confirmBtn.textContent = confirmText;
    confirmBtn.classList.toggle('btn-delete', isDanger);
    confirmBtn.classList.toggle('btn-edit', !isDanger);
    document.getElementById('fbDialogCancel').style.display = onConfirm ? '' : 'none';
    fbDialogAction = onConfirm;
    Fancybox.show([{ src: '#fbDialog', type: 'inline' }]);
}

function showConfirm(message, onConfirm) { openFbDialog(message, 'Sil', true, onConfirm); }
function showAlert(message) { openFbDialog(message, 'Tamam', false, null); }

document.addEventListener('click', function (e) {
    if (e.target.id !== 'fbDialogCancel' && e.target.id !== 'fbDialogConfirm') return;

    var action = e.target.id === 'fbDialogConfirm' ? fbDialogAction : null;
    fbDialogAction = null;
    Fancybox.close();
    if (action) action();
});

document.addEventListener('submit', function (e) {
    var form = e.target;
    if (!form.matches('form[data-confirm]')) return;

    e.preventDefault();
    showConfirm(form.dataset.confirm, function () { form.submit(); });
});

$(document).ready(function() {

    var isSlugManuallyEdited = $('#slugInput').length > 0 && $('#slugInput').val().trim().length > 0;

    function generateSlug(text) {
        if (!text) return '';
        var trMap = { 'ç': 'c', 'Ç': 'c', 'ğ': 'g', 'Ğ': 'g', 'ş': 's', 'Ş': 's', 'ü': 'u', 'Ü': 'u', 'ı': 'i', 'İ': 'i', 'ö': 'o', 'Ö': 'o' };
        text = text.replace(/[çğşüıöÇĞŞÜİÖ]/g, function (match) { return trMap[match]; });
        return text.toLowerCase().replace(/[^a-z0-9\s-]/g, '').trim().replace(/\s+/g, '-').replace(/-+/g, '-');
    }

    function formatBytes(bytes, decimals) {
        if (!bytes || bytes === 0) return '0 Bytes';
        if (decimals === undefined) decimals = 2;
        var k = 1024;
        var dm = decimals < 0 ? 0 : decimals;
        var sizes = ['Bytes', 'KB', 'MB', 'GB'];
        var i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
    }

    function showFileError(msg) {
        $('#fileValidationError').text(msg).show();
        $('.file-wrapper').closest('.form-group').addClass('has-error');
    }

    function clearFileError() {
        $('#fileValidationError').text('').hide();
        $('.file-wrapper').closest('.form-group').removeClass('has-error');
    }

    function resetPreviewToDefault() {
        var $img = $('#imagePreview');
        var originalSrc = $img.attr('data-original-src');

        if (originalSrc && originalSrc.trim() !== '') {
            $img.attr('src', originalSrc);
            $('#previewBadge').html('<i class="fa-solid fa-image"></i> Mevcut Görsel');
            $('#previewInfo').text('Kayıtlı görsel kullanılıyor');
            $('#btnRemoveImage').hide();
            $('#imagePreviewWrapper').show();
        } else {
            $img.attr('src', '#');
            $('#previewInfo').text('');
            $('#imagePreviewWrapper').slideUp(200);
        }
    }

    $('.file').on('change', function (e) {
        clearFileError();
        var file = e.target.files[0];
        if (!file) {
            return;
        }

        var validTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
        if (validTypes.indexOf(file.type.toLowerCase()) === -1) {
            showFileError('Lütfen geçerli bir görsel dosyası seçiniz (JPG, PNG, GIF, WEBP).');
            $(this).val('');
            resetPreviewToDefault();
            return;
        }

        var maxSizeInBytes = 5 * 1024 * 1024;
        if (file.size > maxSizeInBytes) {
            showFileError('Görsel boyutu 5 MB\'tan büyük olamaz.');
            $(this).val('');
            resetPreviewToDefault();
            return;
        }

        var reader = new FileReader();
        var $fileInput = $(this);
        reader.onload = function (event) {
            var img = new Image();
            img.src = event.target.result;
            img.onload = function () {
                var requiredWidth = 300;
                var requiredHeight = 300;

                if (this.width !== requiredWidth || this.height !== requiredHeight) {
                    showFileError('Görsel boyutları tam olarak ' + requiredWidth + 'x' + requiredHeight + ' px olmalıdır. Seçilen görsel: ' + this.width + 'x' + this.height + ' px');
                    $fileInput.val('');
                    resetPreviewToDefault();
                    return;
                }

                $('#imagePreview').attr('src', event.target.result);
                $('#previewBadge').html('<i class="fa-solid fa-image"></i> Seçilen Yeni Görsel');
                $('#previewInfo').text(file.name + ' (' + this.width + 'x' + this.height + ' px - ' + formatBytes(file.size) + ')');
                $('#btnRemoveImage').show();
                $('#imagePreviewWrapper').slideDown(200);
            };
        };
        reader.readAsDataURL(file);
    });

    $('#btnRemoveImage').on('click', function () {
        $('.file').val('');
        clearFileError();
        resetPreviewToDefault();
    });

    $('.blog-form, form').on('submit', function (e) {
        var isValid = true;
        var $firstError = null;

        $(this).find('.form-group.requiredField').each(function () {
            var $group = $(this);
            var $errorSpan = $group.children('.validation-error-msg');
            if ($errorSpan.length === 0) {
                $errorSpan = $('<span class="validation-error-msg"></span>').appendTo($group);
            }

            var labelText = $group.find('label').first().clone().children().remove().end().text().trim();
            if (!labelText) labelText = 'Bu alan';

            var isFieldEmpty = false;

            var $fileInput = $group.find('.file');
            if ($fileInput.length > 0) {
                var fileVal = $fileInput.val();
                var $img = $('#imagePreview');
                var originalSrc = $img.attr('data-original-src');
                var hasExistingImage = originalSrc && originalSrc.trim() !== '';

                if (!fileVal && !hasExistingImage) {
                    isFieldEmpty = true;
                }
            } else if ($summernote.length > 0) {
                var code = $summernote.summernote('code');
                var plainText = $('<div>').html(code).text().trim();
                if (!plainText) {
                    isFieldEmpty = true;
                }
            } else {
                var $input = $group.find('input[type="text"], textarea, select');
                if ($input.length > 0) {
                    var val = $input.val();
                    if (!val || (typeof val === 'string' && val.trim() === '')) {
                        isFieldEmpty = true;
                    }
                }
            }

            if (isFieldEmpty) {
                $group.addClass('has-error');
                if ($fileInput.length > 0) {
                    showFileError('Lütfen bir küçük resim (Thumbnail) seçiniz.');
                } else {
                    $errorSpan.text(labelText + ' alanı boş bırakılamaz.').show();
                }
                isValid = false;
                if (!$firstError) {
                    $firstError = $group;
                }
            } else {
                $group.removeClass('has-error');
                if ($fileInput.length > 0) {
                    clearFileError();
                } else {
                    $errorSpan.text('').hide();
                }
            }
        });

        if (!isValid) {
            e.preventDefault();
            if ($firstError && $firstError.length) {
                $('html, body').animate({
                    scrollTop: $firstError.offset().top - 120
                }, 300);
            }
        }
    });

    $(document).on('input change', '.form-group.requiredField input, .form-group.requiredField textarea, .form-group.requiredField select', function () {
        var $group = $(this).closest('.form-group.requiredField');
        $group.removeClass('has-error');
        $group.children('.validation-error-msg').text('').hide();
    });

    $('#slugInput').on('input', function () {
        isSlugManuallyEdited = $(this).val().trim().length > 0;
    });

    $('#titleInput').on('input', function () {
        if (!isSlugManuallyEdited) {
            var titleVal = $(this).val();
            $('#slugInput').val(generateSlug(titleVal));
        }
    });

    $('.select2-select').select2({
        placeholder: "Kategori aramak için yazınız",
        allowClear: false,
        width: '100%'
    });

    $('#summernote').summernote({
        placeholder: 'Blog içeriğinizi buraya yazabilirsiniz...',
        tabsize: 2,
        height: 350,
        toolbar: [
            ['style', ['style']],
            ['font', ['bold', 'italic', 'underline', 'clear', 'strikethrough']],
            ['color', ['color']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['table', ['table']],
            ['insert', ['link', 'picture', 'video']],
            ['view', ['fullscreen', 'codeview', 'help']]
        ],
        callbacks: {
            onImageUpload: function (files) {
                for (var i = 0; i < files.length; i++) {
                    uploadEditorImage(files[i]);
                }
            },
            onPaste: function (e) {
                var clipboardData = e.originalEvent.clipboardData;
                if (clipboardData && clipboardData.items && clipboardData.items.length) {
                    for (var i = 0; i < clipboardData.items.length; i++) {
                        var item = clipboardData.items[i];
                        if (item.type.indexOf('image') !== -1) {
                            var file = item.getAsFile();
                            if (file) {
                                uploadEditorImage(file);
                                e.preventDefault();
                                break;
                            }
                        }
                    }
                }
            }
        }
    });

    function uploadEditorImage(file) {
        var formData = new FormData();
        formData.append('file', file);

        var currentSlug = $('#slugInput').val().trim() || generateSlug($('#titleInput').val().trim());
        formData.append('slug', currentSlug);

        $.ajax({
            url: '/blgpnl/yazilar/upload-editor-image',
            type: 'POST',
            headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').first().val() || '' },
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                    $('#summernote').summernote('insertImage', response.url);
                } else {
                    showAlert('Görsel yüklenirken hata oluştu: ' + response.message);
                }
            },
            error: function () {
                showAlert('Görsel yüklenirken sunucu hatası oluştu.');
            }
        });
    }


    var searchTimer = null;
    var activeRequest = null;

    function escapeHtml(str) {
        if (!str) return '';
        return String(str).replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#039;");
    }

    function performSearch(query) {
        if (activeRequest) {
            activeRequest.abort();
        }

        $('#searchSpinner').show();

        activeRequest = $.ajax({
            url: '/blgpnl/yazilar/ara',
            type: 'GET',
            data: { q: query },
            success: function (data) {
                var $tbody = $('#blogTableBody');
                $tbody.empty();

                if (data && data.length > 0) {
                    var token = $('input[name="__RequestVerificationToken"]').first().val() || '';
                    $.each(data, function (index, item) {
                        var rowHtml = '<tr>' +
                            '<td>' + escapeHtml(item.title) + '</td>' +
                            '<td>' + escapeHtml(item.dateFormatted) + '</td>' +
                            '<td>' + escapeHtml(item.statusText) + '</td>' +
                            '<td>' + item.viewCount + '</td>' +
                            '<td><a href="/blgpnl/yazilar/duzenle/' + encodeURIComponent(item.slug) + '#yorumlar">' + item.commentCount + '</a></td>' +
                            '<td class="row">' +
                                '<a href="/blgpnl/yazilar/duzenle/' + encodeURIComponent(item.slug) + '" class="btn btn-edit">Düzenle</a> ' +
                                '<form method="post" action="/blgpnl/yazilar/sil/' + item.id + '" style="display:inline;" data-confirm="Bu blog yazısını silmek istediğinize emin misiniz?">' +
                                    '<input type="hidden" name="__RequestVerificationToken" value="' + token + '" />' +
                                    '<button type="submit" class="btn btn-delete">Sil</button>' +
                                '</form>' +
                            '</td>' +
                        '</tr>';
                        $tbody.append(rowHtml);
                    });
                } else {
                    $tbody.append('<tr><td colspan="6" style="text-align: center; padding: 20px; color: #666;">Aramaya uygun blog yazısı bulunamadı.</td></tr>');
                }
            },
            error: function (xhr, status, error) {
                if (status !== 'abort') {
                    console.error("Arama sırasında bir hata oluştu:", error);
                }
            },
            complete: function () {
                $('#searchSpinner').hide();
                activeRequest = null;
            }
        });
    }

    $('#blogSearchInput').on('input', function () {
        var query = $(this).val();

        clearTimeout(searchTimer);
        searchTimer = setTimeout(function () {
            performSearch(query);
        }, 300);
    });
})