$(document).ready(function() {
  
  // Header Navigation Active State
  $('.header').on('click', 'nav a[href="javascript:;"]', function(e) {
    e.preventDefault();
    $('.header nav a').removeClass('active');
    $(this).addClass('active');
  });

  // Global Auth Triggers
  $(document).on('click', 'a, button', function(e) {
    const btnText = $(this).text().trim();
    if (btnText === 'Sign In' || btnText === 'Log In') {
      e.preventDefault();
      window.location.href = 'login.html';
    } else if (btnText === 'Get Started' || btnText === 'Create Account') {
      // Don't trigger if it's the submit button of the register form
      if ($(this).attr('type') !== 'submit') {
        e.preventDefault();
        window.location.href = 'register.html';
      }
    }
  });

  // Password visibility toggle
  $(document).on('click', '.btn-toggle-pwd', function(e) {
    e.preventDefault();
    const input = $(this).siblings('input');
    if (input.attr('type') === 'password') {
      input.attr('type', 'text');
    } else {
      input.attr('type', 'password');
    }
  });

  // Auth Forms Submission (Mock)
  $(document).on('submit', '.auth-form', function(e) {
    e.preventDefault();
    console.log($(this).attr('id') + ' submitted');
    // If it's login form, we could show error just as an example:
    if ($(this).attr('id') === 'login-form') {
      $('#login-error').slideDown();
    }
  });

  // Feed Filters
  $('.feed').on('click', '.filters button', function(e) {
    e.preventDefault();
    $('.feed .filters button').removeClass('active');
    $(this).addClass('active');
  });

  // Sidebar Links
  $('.sidebar').on('click', 'a[href="javascript:;"]', function(e) {
    e.preventDefault();
    console.log('Sidebar link clicked:', $(this).text().trim());
  });

  // Newsletter Submit
  $('.sidebar').on('submit', 'form', function(e) {
    e.preventDefault();
    const input = $(this).find('input[type="email"]');
    if (input.val()) {
      console.log('Subscribed with:', input.val());
      input.val(''); // clear input
    }
  });

});
