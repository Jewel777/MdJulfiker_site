/* ===== site.js — v11 (stable, smooth, production-safe) ===== */
(function () {
  "use strict";

  // ---------- Helpers ----------
  function $(id) { return document.getElementById(id); }
  const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

  // ---------- Persistent day / night theme ----------
  document.addEventListener("DOMContentLoaded", function () {
    const toggle = $("themeToggle");
    if (!toggle) return;

    function syncThemeControl() {
      const dark = document.documentElement.getAttribute("data-theme") === "dark";
      toggle.setAttribute("aria-pressed", String(dark));
      toggle.setAttribute("aria-label", dark ? "Switch to light mode" : "Switch to dark mode");
      const label = toggle.querySelector(".theme-label");
      if (label) label.textContent = dark ? "Day" : "Night";
      const themeMeta = document.querySelector('meta[name="theme-color"]');
      if (themeMeta) themeMeta.setAttribute("content", dark ? "#090d16" : "#edf1f7");
    }

    syncThemeControl();
    toggle.addEventListener("click", function () {
      const next = document.documentElement.getAttribute("data-theme") === "dark" ? "light" : "dark";
      document.documentElement.setAttribute("data-theme", next);
      try { localStorage.setItem("portfolio-theme", next); } catch (_) { /* no-op */ }
      syncThemeControl();
      initParticles();
    });
  });

  // ---------- Spinner (hide safely) ----------
  function hideSpinner() {
    const s = $("loading-spinner");
    if (s) s.style.display = "none";
  }
  // Hide ASAP after DOM, and also after full load, plus a fallback timeout
  document.addEventListener("DOMContentLoaded", hideSpinner);
  window.addEventListener("load", hideSpinner);
  setTimeout(hideSpinner, 2000);

  // ---------- Resume preview lifecycle ----------
  // Load the PDF only while the modal is open. This prevents the embedded
  // viewer from sitting above modal controls and releases it cleanly on close.
  document.addEventListener("DOMContentLoaded", function () {
    const modal = $("resumePreview");
    if (!modal || typeof bootstrap === "undefined") return;

    // Bootstrap modals must live at body level; otherwise the main content's
    // stacking context can place the fixed navigation above the modal header.
    if (modal.parentElement !== document.body) document.body.appendChild(modal);

    const frame = modal.querySelector(".resume-iframe");
    const closeControls = modal.querySelectorAll('[data-bs-dismiss="modal"]');

    modal.addEventListener("shown.bs.modal", function () {
      if (frame && frame.dataset.src && frame.getAttribute("src") !== frame.dataset.src) {
        frame.setAttribute("src", frame.dataset.src);
      }
      const close = $("resumePreviewClose");
      if (close) close.focus();
    });

    modal.addEventListener("hidden.bs.modal", function () {
      if (frame) frame.setAttribute("src", "about:blank");
    });

    closeControls.forEach(function (control) {
      control.addEventListener("click", function () {
        bootstrap.Modal.getOrCreateInstance(modal).hide();
      });
    });
  });

  // ---------- Scroll to top (if you call it from HTML) ----------
  window.scrollToTop = function () {
    window.scrollTo({ top: 0, behavior: reduceMotion ? "auto" : "smooth" });
  };

  // ---------- Chatbot toggle (if you call it from HTML) ----------
  window.toggleChatbot = function () {
    const modal = $("chatbotModal");
    if (!modal) return;

    // default "none" if empty
    const current = (modal.style.display || "none");
    modal.style.display = (current === "none") ? "block" : "none";
  };

  // ---------- Theme-aware particles (safe) ----------
  function particlePalette() {
    const dark = document.documentElement.getAttribute("data-theme") === "dark";
    return dark
      ? { node: "#d7d0ff", line: "#8fa8ff", lineOpacity: 0.48, nodeOpacity: 0.72 }
      : { node: "#52637a", line: "#7185a0", lineOpacity: 0.38, nodeOpacity: 0.58 };
  }

  function destroyParticles() {
    try {
      if (Array.isArray(window.pJSDom)) {
        window.pJSDom.slice().forEach(function (instance) {
          if (instance && instance.pJS && instance.pJS.fn && instance.pJS.fn.vendors) {
            instance.pJS.fn.vendors.destroypJS();
          }
        });
        window.pJSDom.length = 0;
      }
      const host = $("particles-js");
      if (host) host.replaceChildren();
    } catch (_) { /* safe cleanup */ }
  }

  function initParticles() {
    try {
      if (reduceMotion || typeof particlesJS !== "function") return;
      const palette = particlePalette();
      destroyParticles();
      const host = $("particles-js");
      if (host) {
        host.dataset.particleTheme = document.documentElement.getAttribute("data-theme") || "light";
        host.dataset.particleNode = palette.node;
        host.dataset.particleLine = palette.line;
      }
      particlesJS("particles-js", {
        particles: {
          number: { value: 65 },
          size: { value: 2 },
          move: { speed: 1.1 },
          line_linked: { enable: true, color: palette.line, opacity: palette.lineOpacity },
          color: { value: palette.node },
          opacity: { value: palette.nodeOpacity }
        }
      });
    } catch (e) {
      console.warn("particlesJS init failed:", e);
    }
  }

  document.addEventListener("DOMContentLoaded", function () {
    initParticles();
  });

  // ---------- Typed.js (best branding strings) ----------
  document.addEventListener("DOMContentLoaded", function () {
    const el = $("typed");
    if (!el) return;

    if (reduceMotion) {
      el.textContent = "Secure .NET Systems Engineer";
      return;
    }

    try {
      if (typeof Typed !== "function") return;

      new Typed("#typed", {
        strings: [
          "Secure .NET Systems Engineer",
          "Software Programmer Analyst (WV DHHR)",
          "Cybersecurity-Focused Application Developer",
          "Building Resilient Public-Sector Platforms"
        ],
        typeSpeed: 55,
        backSpeed: 32,
        backDelay: 900,     // ✅ prevents awkward mid-word switching
        startDelay: 250,
        smartBackspace: true,
        loop: true,
        showCursor: true
      });
    } catch (e) {
      console.warn("Typed init failed:", e);
    }
  });

  // Close the mobile menu after navigating to a one-page section.
  document.addEventListener("DOMContentLoaded", function () {
    const menu = $("navbarContent");
    if (!menu || typeof bootstrap === "undefined") return;

    menu.querySelectorAll('a[href*="#"]').forEach((link) => {
      link.addEventListener("click", () => {
        if (menu.classList.contains("show")) {
          bootstrap.Collapse.getOrCreateInstance(menu, { toggle: false }).hide();
        }
      });
    });

    // Protect every new-tab link, including links rendered by partial views.
    document.querySelectorAll('a[target="_blank"]').forEach((link) => {
      link.relList.add("noopener", "noreferrer");
    });

    // Keep the sculpted navigation state synchronized with the visible section.
    const navItems = Array.from(document.querySelectorAll("#mainNavbar [data-nav]"));
    const sections = navItems
      .map((item) => document.getElementById(item.dataset.nav))
      .filter(Boolean);

    function setActiveNav(id) {
      navItems.forEach((item) => {
        const active = item.dataset.nav === id;
        item.classList.toggle("active", active);
        if (active) item.setAttribute("aria-current", "page");
        else item.removeAttribute("aria-current");
      });
    }

    navItems.forEach((item) => {
      item.addEventListener("click", () => setActiveNav(item.dataset.nav));
    });

    if (sections.length && "IntersectionObserver" in window) {
      const sectionObserver = new IntersectionObserver((entries) => {
        const visible = entries
          .filter((entry) => entry.isIntersecting)
          .sort((a, b) => Math.abs(a.boundingClientRect.top) - Math.abs(b.boundingClientRect.top));
        if (visible[0]) setActiveNav(visible[0].target.id);
      }, { rootMargin: "-20% 0px -65% 0px", threshold: 0 });

      sections.forEach((section) => sectionObserver.observe(section));
    }

    setActiveNav((window.location.hash || "#home").slice(1));
  });

  // ---------- Counters (animate only when visible) ----------
  document.addEventListener("DOMContentLoaded", function () {
    const counters = document.querySelectorAll(".counter");
    if (!counters.length) return;

    function animateCounter(el) {
      const target = Number(el.dataset.target || 0);
      if (reduceMotion) {
        el.textContent = target.toLocaleString();
        return;
      }
      let current = 0;

      // Smooth steps: about ~80 frames
      const step = Math.max(1, Math.ceil(target / 80));

      function tick() {
        current += step;
        if (current >= target) {
          el.textContent = target.toLocaleString();
          return;
        }
        el.textContent = current.toLocaleString();
        requestAnimationFrame(tick);
      }

      tick();
    }

    const io = new IntersectionObserver((entries) => {
      entries.forEach((entry) => {
        if (entry.isIntersecting) {
          animateCounter(entry.target);
          io.unobserve(entry.target);
        }
      });
    }, { threshold: 0.5 });

    counters.forEach((c) => {
      c.textContent = "0";
      io.observe(c);
    });
  });

})();

