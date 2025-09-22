
(function () {
  var STORAGE_KEY = 'acFilter';

  function initAccomplishments() {
    var grid  = document.getElementById('accomplishment-grid');
    if (!grid) return;

    var btnBar = document.getElementById('accomplishment-filter-buttons'); // desktop
    var select = document.getElementById('acFilterSelect');               // mobile

    var buttons = btnBar ? Array.from(btnBar.querySelectorAll('.filter-btn')) : [];
    var cards = Array.from(grid.children).filter(function (el) {
      return el.classList && el.classList.contains('col-md-6');
    });

    function applyFilter(val) {
      var f = (val || 'all').toLowerCase();
      cards.forEach(function (card) {
        var match = (f === 'all') || card.classList.contains(f);
        card.style.display = match ? '' : 'none';
      });
    }

    function setActiveButtonByValue(val) {
      if (!buttons.length) return;
      buttons.forEach(function (b) { b.classList.remove('active'); });
      var match = buttons.find(function (b) {
        return (b.getAttribute('data-filter') || '').toLowerCase() === (val || 'all').toLowerCase();
      });
      (match || buttons[0]).classList.add('active');
    }

    function setSelectValue(val) {
      if (!select) return;
      var found = Array.from(select.options).some(function (o) { return o.value === val; });
      select.value = found ? val : 'all';
    }

    function save(val){
      try { sessionStorage.setItem(STORAGE_KEY, val); } catch (e) {}
    }
    function load(){
      try { return sessionStorage.getItem(STORAGE_KEY) || 'all'; } catch (e) { return 'all'; }
    }

    // Read initial from ?ac=... (if present) OR sessionStorage, then STRIP ?ac from URL
    function initialValue() {
      var v = null;
      try {
        var url = new URL(window.location.href);
        if (url.searchParams.has('ac')) {
          v = url.searchParams.get('ac') || 'all';
          url.searchParams.delete('ac');
          var cleaned = url.toString().replace(/\?$/, ''); // avoid trailing '?'
          history.replaceState(null, '', cleaned);
        }
      } catch (e) {}
      return v || load() || 'all';
    }

    function updateAll(val, shouldSave) {
      applyFilter(val);
      setActiveButtonByValue(val);
      setSelectValue(val);
      if (shouldSave !== false) save(val);
    }

    window.acApply = function (v) { updateAll(v || 'all'); };

    // Desktop: button clicks
    if (btnBar) {
      btnBar.addEventListener('click', function (e) {
        var btn = e.target.closest('.filter-btn');
        if (!btn) return;
        e.preventDefault();
        updateAll(btn.getAttribute('data-filter') || 'all');
      });
    }

    // Mobile: dropdown changes (robust on iOS)
    if (select) {
      var handler = function () { updateAll(select.value || 'all'); };
      select.addEventListener('change', handler);
      select.addEventListener('input', handler);
      select.addEventListener('blur', handler, true);
    }

    // Init (no URL writes)
    updateAll(initialValue(), false);
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initAccomplishments);
  } else {
    initAccomplishments();
  }
})();
