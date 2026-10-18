document.addEventListener('DOMContentLoaded', function () {
    const btn = document.getElementById('bookmark-btn');
    if (!btn) return;

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    btn.addEventListener('click', async function () {
        if (btn.disabled) return;
        btn.disabled = true;

        try {
            const res = await fetch(btn.dataset.url, {
                method: 'POST',
                headers: { 'RequestVerificationToken': token }
            });

            if (res.status === 401) {
                window.location.href = '/giris-yap?geldigiSayfa=' + encodeURIComponent(window.location.pathname);
                return;
            }
            if (!res.ok) throw new Error(res.status);

            const data = await res.json();
            const label = data.bookmarked ? 'Kayıtlardan çıkar' : 'Kaydet';
            btn.classList.toggle('is-active', data.bookmarked);
            btn.setAttribute('aria-pressed', data.bookmarked ? 'true' : 'false');
            btn.setAttribute('aria-label', label);
            btn.title = label;
        } catch (e) {
            btn.title = 'Kaydedilemedi, tekrar deneyin';
        } finally {
            btn.disabled = false;
        }
    });
});

document.addEventListener('DOMContentLoaded', function () {
    const btn = document.getElementById('clap-btn');
    if (!btn) return;

    const count = document.getElementById('clap-count');
    const token = document.querySelector('.reactions input[name="__RequestVerificationToken"]').value;
    let busy = false;

    function render(liked, likeCount) {
        btn.classList.toggle('is-active', liked);
        btn.setAttribute('aria-pressed', liked ? 'true' : 'false');
        btn.title = liked ? 'Alkışı geri al' : 'Alkışla';
        count.textContent = likeCount;
    }

    btn.addEventListener('click', async function () {
        if (busy) return;
        busy = true;
        btn.disabled = true;

        try {
            const res = await fetch(btn.dataset.url, {
                method: 'POST',
                headers: { 'RequestVerificationToken': token }
            });

            if (res.status === 401) {
                window.location.href = '/giris-yap?geldigiSayfa=' + encodeURIComponent(window.location.pathname);
                return;
            }
            if (!res.ok) throw new Error(res.status);

            const data = await res.json();
            render(data.liked, data.likeCount);
        } catch (e) {
            btn.title = 'Alkış kaydedilemedi, tekrar deneyin';
        } finally {
            busy = false;
            btn.disabled = false;
        }
    });
});

document.addEventListener('DOMContentLoaded', function () {
    const section = document.getElementById('comments');
    if (!section) return;

    const template = document.getElementById('reply-form-template');
    const tokenInput = section.querySelector('input[name="__RequestVerificationToken"]');
    let activeReply = null;

    function guardEmpty(form) {
        const textarea = form.querySelector('textarea');
        const error = form.querySelector('.comment-form__error');

        textarea.addEventListener('input', function () { error.hidden = true; });
        form.addEventListener('submit', function (e) {
            if (!textarea.value.trim()) {
                e.preventDefault();
                error.hidden = false;
                textarea.focus();
                return;
            }
            form.querySelector('button[type="submit"]').disabled = true;
        });
    }

    function closeReply() {
        if (!activeReply) return;
        activeReply.form.remove();
        activeReply.button.setAttribute('aria-expanded', 'false');
        activeReply = null;
    }

    function openReply(button) {
        const form = template.content.firstElementChild.cloneNode(true);
        form.querySelector('input[name="ParentCommentId"]').value = button.dataset.commentId;
        form.querySelector('.comment-form__cancel').addEventListener('click', function () {
            closeReply();
            button.focus();
        });
        guardEmpty(form);

        button.closest('.foot').after(form);
        button.setAttribute('aria-expanded', 'true');
        activeReply = { form: form, button: button };
        form.querySelector('textarea').focus();
    }

    function closeMenus(except) {
        section.querySelectorAll('.comment-menu__toggle[aria-expanded="true"]').forEach(function (toggle) {
            if (toggle === except) return;
            toggle.setAttribute('aria-expanded', 'false');
            toggle.nextElementSibling.hidden = true;
        });
    }

    async function toggleLike(button) {
        button.disabled = true;
        try {
            const res = await fetch(button.dataset.url, {
                method: 'POST',
                headers: { 'RequestVerificationToken': tokenInput.value }
            });

            if (res.status === 401) {
                window.location.href = '/giris-yap?geldigiSayfa=' + encodeURIComponent(window.location.pathname);
                return;
            }
            if (!res.ok) throw new Error(res.status);

            const data = await res.json();
            button.classList.toggle('is-active', data.liked);
            button.setAttribute('aria-pressed', data.liked ? 'true' : 'false');
            button.querySelector('span').textContent = data.likeCount;
            button.removeAttribute('title');
        } catch (e) {
            button.title = 'Beğeni kaydedilemedi, tekrar deneyin';
        } finally {
            button.disabled = false;
        }
    }

    const rootForm = section.querySelector('.comment-form');
    if (rootForm) guardEmpty(rootForm);

    section.addEventListener('click', function (e) {
        if (e.target.closest('.comment-menu__list')) return;

        const replyButton = e.target.closest('.comment-reply');
        if (replyButton && template) {
            const isOpen = activeReply && activeReply.button === replyButton;
            closeReply();
            if (!isOpen) openReply(replyButton);
            return;
        }

        const likeButton = e.target.closest('.comment-like');
        if (likeButton) {
            if (!likeButton.disabled) toggleLike(likeButton);
            return;
        }

        const menuToggle = e.target.closest('.comment-menu__toggle');
        closeMenus(menuToggle);
        if (menuToggle) {
            const isOpen = menuToggle.getAttribute('aria-expanded') === 'true';
            menuToggle.setAttribute('aria-expanded', isOpen ? 'false' : 'true');
            menuToggle.nextElementSibling.hidden = isOpen;
        }
    });

    document.addEventListener('click', function (e) {
        if (!section.contains(e.target)) closeMenus(null);
    });

    section.addEventListener('submit', function (e) {
        if (e.target.matches('.comment-menu__list') && !confirm('Bu yorumu silmek istediğine emin misin?')) {
            e.preventDefault();
        }
    });
});
