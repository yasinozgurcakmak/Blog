$(document).ready(function() {
  
  $('.header').on('click', 'nav a[href="javascript:;"]', function(e) {
    e.preventDefault();
    $('.header nav a').removeClass('active');
    $(this).addClass('active');
  });

  $('.feed').on('click', '.filters button', function(e) {
    e.preventDefault();
    $('.feed .filters button').removeClass('active');
    $(this).addClass('active');
  });

  $('.sidebar').on('click', 'a[href="javascript:;"]', function(e) {
    e.preventDefault();
    console.log('Sidebar link clicked:', $(this).text().trim());
  });

  $('.sidebar').on('submit', 'form', function(e) {
    e.preventDefault();
    const input = $(this).find('input[type="email"]');
    if (input.val()) {
      console.log('Subscribed with:', input.val());
      input.val(''); 
    }
  });
    const btn = document.getElementById('load-more-btn');
    if (!btn) return;
    const list = document.getElementById('blog-list');
    const sentinel = btn.closest('.load-more');
    let loading = false;

    async function loadNext() {
        if (loading) return;
        loading = true;
        btn.disabled = true;
        btn.textContent = 'Yükleniyor...';

        const page = parseInt(btn.dataset.page, 10);
        const params = new URLSearchParams({ page });
        if (btn.dataset.categoryId) params.set('categoryId', btn.dataset.categoryId);

        try {
            const res = await fetch(btn.dataset.url + '?' + params.toString());
            if (!res.ok) throw new Error(res.status);
            list.insertAdjacentHTML('beforeend', await res.text());

            if (res.headers.get('X-Has-More') === 'true') {
                btn.dataset.page = page + 1;
                btn.disabled = false;
                btn.textContent = 'Daha Fazla Yazı Yükle';
                observer.unobserve(sentinel);
                observer.observe(sentinel);
            } else {
                observer.disconnect();
                sentinel.remove();
            }
        } catch (e) {
            observer.disconnect();
            btn.disabled = false;
            btn.textContent = 'Yüklenemedi, tekrar dene';
        } finally {
            loading = false;
        }
    }

    const observer = new IntersectionObserver(function (entries) {
        if (entries.some(e => e.isIntersecting)) loadNext();
    }, { rootMargin: '300px 0px' });

    observer.observe(sentinel);
    btn.addEventListener('click', loadNext);


    const form = document.getElementById('profile-form');
    if (!form) return;

    const avatar = document.getElementById('avatar-img');
    const fileInput = document.getElementById('profile-image');
    const removeInput = document.getElementById('remove-image');
    const removeBtn = document.getElementById('btn-remove-image');
    const nameInput = document.getElementById('profile-name');
    const surnameInput = document.getElementById('profile-surname');

    function fallbackAvatarUrl() {
        const fullName = (nameInput.value + ' ' + surnameInput.value).trim();
        return avatar.dataset.fallbackBase + encodeURIComponent(fullName);
    }

    function showFallback() {
        avatar.dataset.hasImage = 'false';
        avatar.src = fallbackAvatarUrl();
    }

    document.getElementById('btn-upload').addEventListener('click', () => fileInput.click());

    fileInput.addEventListener('change', function () {
        const file = fileInput.files[0];
        if (!file) return;
        avatar.src = URL.createObjectURL(file);
        avatar.dataset.hasImage = 'true';
        removeInput.value = 'false';
        removeBtn.hidden = false;
    });

    removeBtn.addEventListener('click', function () {
        fileInput.value = '';
        removeInput.value = 'true';
        removeBtn.hidden = true;
        showFallback();
    });

    let avatarTimer;
    [nameInput, surnameInput].forEach(input => input.addEventListener('input', function () {
        if (avatar.dataset.hasImage === 'true') return;
        clearTimeout(avatarTimer);
        avatarTimer = setTimeout(() => { avatar.src = fallbackAvatarUrl(); }, 400);
    }));

    const bio = document.getElementById('profile-bio');
    const bioCounter = document.getElementById('bio-counter');
    bio.addEventListener('input', function () {
        bioCounter.textContent = `${bio.value.length} / ${bio.maxLength} karakter`;
    });

    form.addEventListener('submit', function () {
        const saveBtn = document.getElementById('save-btn');
        document.getElementById('save-label').textContent = 'Kaydediliyor...';
        document.getElementById('save-icon').textContent = '↻';
        saveBtn.disabled = true;
        saveBtn.style.opacity = '0.7';
    });
});
