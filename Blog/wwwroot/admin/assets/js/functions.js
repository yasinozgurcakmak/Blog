document.addEventListener('DOMContentLoaded', function () {
    Fancybox.bind("[data-fancybox]", {});
});


document.getElementById('profileImageInput').addEventListener('change', function () {
    const file = this.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = function (e) {
        document.getElementById('profilePreview').src = e.target.result;
    };
    reader.readAsDataURL(file);
});


