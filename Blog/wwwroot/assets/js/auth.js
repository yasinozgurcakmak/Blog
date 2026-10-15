document.addEventListener('DOMContentLoaded', function () {

  document.querySelectorAll('.btn-toggle-pwd').forEach(function (btn) {
    btn.addEventListener('click', function (e) {
      e.preventDefault();
      const input = btn.parentElement.querySelector('input');
      input.type = input.type === 'password' ? 'text' : 'password';
    });
  });

  const resendBtn = document.getElementById('resend-btn');
  if (resendBtn) {
    let secondsLeft = parseInt(resendBtn.dataset.cooldown, 10) || 60;
    const tick = function () {
      if (secondsLeft <= 0) {
        resendBtn.disabled = false;
        resendBtn.textContent = 'Tekrar gönder';
        return;
      }
      resendBtn.textContent = `Tekrar gönder (${secondsLeft} sn)`;
      secondsLeft--;
      setTimeout(tick, 1000);
    };
    tick();
  }

  const password = document.getElementById('reg-password');
  const confirm = document.getElementById('reg-password-confirm');
  if (!password || !confirm) return;

  const rules = [
    { label: '8+ karakter', test: v => v.length >= 8 },
    { label: 'küçük harf', test: v => /[a-z]/.test(v) },
    { label: 'büyük harf', test: v => /[A-Z]/.test(v) },
    { label: 'rakam', test: v => /[0-9]/.test(v) },
    { label: 'sembol', test: v => /[^a-zA-Z0-9]/.test(v) }
  ];
  const levels = [
    { text: '', hint: '8+ karakter, büyük/küçük harf, rakam ve sembol' },
    { text: 'Zayıf şifre' },
    { text: 'Orta şifre' },
    { text: 'İyi şifre' },
    { text: 'Güçlü şifre' }
  ];

  const meter = document.getElementById('strength-meter');
  const bars = meter.querySelectorAll('.bar');
  const status = document.getElementById('pwd-status');
  const hint = document.getElementById('strength-hint');
  const percent = document.getElementById('strength-percent');
  const confirmStatus = document.getElementById('confirm-status');

  function getLevel(passed, value) {
    if (!value) return 0;
    if (passed === rules.length) return 4;
    if (passed === 4) return 3;
    if (passed === 3) return 2;
    return 1;
  }

  function updateStrength() {
    const value = password.value;
    const missing = rules.filter(r => !r.test(value)).map(r => r.label);
    const passed = rules.length - missing.length;
    const level = getLevel(passed, value);

    meter.dataset.level = level;
    status.dataset.level = level;
    bars.forEach((bar, i) => bar.classList.toggle('bar--active', i < level));
    status.textContent = levels[level].text;
    percent.textContent = value ? Math.round(passed / rules.length * 100) + '% Güvenli' : '';
    hint.textContent = !value ? levels[0].hint : missing.length ? 'Eksik: ' + missing.join(', ') : 'Tüm kriterler karşılandı';

    password.setCustomValidity(value && missing.length ? 'Şifre şu kriterleri karşılamalı: ' + missing.join(', ') : '');
  }

  function updateConfirm() {
    if (!confirm.value) {
      confirmStatus.textContent = '';
      confirmStatus.dataset.level = 0;
      confirm.setCustomValidity('');
      return;
    }
    const match = confirm.value === password.value;
    confirmStatus.textContent = match ? 'Şifreler eşleşiyor' : 'Şifreler eşleşmiyor';
    confirmStatus.dataset.level = match ? 4 : 1;
    confirm.setCustomValidity(match ? '' : 'Şifreler eşleşmiyor.');
  }

  password.addEventListener('input', function () { updateStrength(); updateConfirm(); });
  confirm.addEventListener('input', updateConfirm);
  updateStrength();
});
