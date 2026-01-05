/* ===== site.js — v11 (stable, smooth, production-safe) ===== */
(function () {
  "use strict";

  // ---------- Helpers ----------
  function $(id) { return document.getElementById(id); }

  // ---------- Spinner (hide safely) ----------
  function hideSpinner() {
    const s = $("loading-spinner");
    if (s) s.style.display = "none";
  }
  // Hide ASAP after DOM, and also after full load, plus a fallback timeout
  document.addEventListener("DOMContentLoaded", hideSpinner);
  window.addEventListener("load", hideSpinner);
  setTimeout(hideSpinner, 2000);

  // ---------- Scroll to top (if you call it from HTML) ----------
  window.scrollToTop = function () {
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  // ---------- Chatbot toggle (if you call it from HTML) ----------
  window.toggleChatbot = function () {
    const modal = $("chatbotModal");
    if (!modal) return;

    // default "none" if empty
    const current = (modal.style.display || "none");
    modal.style.display = (current === "none") ? "block" : "none";
  };

  // ---------- Particles (safe) ----------
  document.addEventListener("DOMContentLoaded", function () {
    try {
      if (typeof particlesJS !== "function") return;
      particlesJS("particles-js", {
        particles: {
          number: { value: 65 },
          size: { value: 2 },
          move: { speed: 1.1 },
          line_linked: { enable: true },
          color: { value: "#ffffff" }
        }
      });
    } catch (e) {
      console.warn("particlesJS init failed:", e);
    }
  });

  // ---------- Typed.js (best branding strings) ----------
  document.addEventListener("DOMContentLoaded", function () {
    const el = $("typed");
    if (!el) return;

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

  // ---------- Counters (animate only when visible) ----------
  document.addEventListener("DOMContentLoaded", function () {
    const counters = document.querySelectorAll(".counter");
    if (!counters.length) return;

    function animateCounter(el) {
      const target = Number(el.dataset.target || 0);
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
