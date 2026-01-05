<script>
    window.addEventListener('load', () => {
        document.getElementById('loading-spinner').style.display = 'none';
    });

    function scrollToTop() {
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }

    function toggleChatbot() {
        var modal = document.getElementById('chatbotModal');
    modal.style.display = (modal.style.display === 'none') ? 'block' : 'none';
    }

    // Particles background
    particlesJS("particles-js", {
        "particles": {
        "number": {"value": 60 },
    "size": {"value": 2 },
    "move": {"speed": 1.5 },
    "line_linked": {"enable": true },
    "color": {"value": "#ffffff" }
        }
    });

    // Typed text animation
    if (document.querySelector('#typed')) {
        new Typed('#typed', {
            strings: ['Welcome to My Portfolio', 'Explore My Work', 'Let\'s Connect!'],
            typeSpeed: 30,
            backSpeed: 20,
            loop: true
        });
    }

    // Counter Animation
    document.addEventListener('DOMContentLoaded', function () {
        const counters = document.querySelectorAll('.counter');
        counters.forEach(counter => {
        counter.innerText = '0';
            const updateCounter = () => {
                const target = +counter.getAttribute('data-target');
    const current = +counter.innerText;
    const increment = target / 100;
    if (current < target) {
        counter.innerText = Math.ceil(current + increment);
    setTimeout(updateCounter, 20);
                } else {
        counter.innerText = target;
                }
            };
    updateCounter();
        });
    });
</script>
