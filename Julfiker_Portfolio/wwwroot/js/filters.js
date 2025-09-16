// Accomplishments category filter (works inside partials)
(function () {
  function initAccomplishments() {
    var bar   = document.getElementById('accomplishment-filter');
    var grid  = document.getElementById('accomplishment-grid');
    if (!bar || !grid) return;

    var buttons = Array.from(bar.querySelectorAll('.filter-btn'));
    // safer than :scope for broad browser support
    var cards = Array.from(grid.children).filter(function (el) {
      return el.classList && el.classList.contains('col-md-6');
    });

    function setActive(btn) {
      buttons.forEach(function (b) { b.classList.remove('active'); });
      btn.classList.add('active');
    }

    function applyFilter(filter) {
      var f = (filter || 'all').toLowerCase();
      cards.forEach(function (card) {
        var match = (f === 'all') || card.classList.contains(f);
        card.style.display = match ? '' : 'none';
      });
    }

    bar.addEventListener('click', function (e) {
      var btn = e.target.closest('.filter-btn');
      if (!btn) return;
      e.preventDefault();
      var filter = btn.getAttribute('data-filter') || 'all';
      setActive(btn);
      applyFilter(filter);

      // Persist selection in URL (?ac=filter)
      try {
        var url = new URL(location.href);
        url.searchParams.set('ac', filter);
        history.replaceState(null, '', url.toString());
      } catch (_) { /* no-op */ }
    });

    // Init from URL (?ac=…) or default to 'all'
    var init = (new URLSearchParams(location.search).get('ac')) || 'all';
    var initBtn = buttons.find(function (b) {
      return (b.getAttribute('data-filter') || '').toLowerCase() === init.toLowerCase();
    }) || buttons[0];

    if (initBtn) {
      setActive(initBtn);
      applyFilter(initBtn.getAttribute('data-filter'));
    } else {
      applyFilter('all');
    }
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initAccomplishments);
  } else {
    initAccomplishments();
  }
})();
