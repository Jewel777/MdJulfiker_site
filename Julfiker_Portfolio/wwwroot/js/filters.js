// Accomplishments filter: desktop buttons + mobile dropdown, hardened for iOS.
(function () {
  function initAccomplishments() {
    var grid  = document.getElementById('accomplishment-grid');
    if (!grid) return;

    var btnBar = document.getElementById('accomplishment-filter-buttons'); // desktop
    var select = document.getElementById('acFilterSelect');               // mobile

    var buttons = btnBar ? Array.from(btnBar.querySelectorAll('.filter-btn')) : [];
    var cards = Array.from(grid.children).filter(function (el) {
      return el.classList && el.classList.contains('col-md-6');
    });

    function setActiveButtonByValue(val) {
      if (!buttons.length) return;
      buttons.forEach(function (b) { b.classList.remove('active'); });
      var match = buttons.find(function (b) {
        return (b.getAttribute('data-filter') || '').toLowerCase() === val.toLowerCase();
      });
      (match || buttons[0]).classList.add('active');
    }

    function setSelectValue(val) {
      if (!select) return;
      var found = Array.from(select.options).some(function (o) { return o.value === val; });
      select.value = found ? val : 'all';
    }

    function applyFilter(val) {
      var f = (val || 'all').toLowerCase();
      cards.forEach(function (card) {
        var match = (f === 'all') || card.classList.contains(f);
        card.style.display = match ? '' : 'none';
      });
    }

    function persist(val) {
      try {
        var url = new URL(location.href);
        url.searchParams.set('ac', val);
        history.replaceState(null, '', url.toString());
      } catch (_) { /* no-op */ }
    }

    function updateAll(val) {
      applyFilter(val);
      setActiveButtonByValue(val);
      setSelectValue(val);
      persist(val);
    }

    // Make the handler globally available as a last-resort fallback (used by inline onchange)
    window.acApply = updateAll;

    // Button clicks (desktop)
    if (btnBar) {
      btnBar.addEventListener('click', function (e) {
        var btn = e.target.closest('.filter-btn');
        if (!btn) return;
        e.preventDefault();
        updateAll(btn.getAttribute('data-filter') || 'all');
      });
    }

    // Dropdown changes (mobile) — robust for iOS
    if (select) {
      var handler = function () { updateAll(select.value || 'all'); };
      select.addEventListener('change', handler);
      select.addEventListener('input', handler); // some iOS versions fire 'input' first
      select.addEventListener('blur', handler, true); // ensure we catch selection after closing picker
    }

    // Initialize from ?ac= param or default 'all'
    var initial = (new URLSearchParams(location.search).get('ac')) || 'all';
    updateAll(initial);
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initAccomplishments);
  } else {
    initAccomplishments();
  }
})();
