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
