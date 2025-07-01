(function () {
    'use strict';

    const config = window.loadingConfig || {
        RedirectUrl: '/Dashboard',
        Duration: 3000
    };

    function initializeLoading() {
        console.log('Loading screen initialized');
        console.log('Redirect after:', config.Duration + 'ms', 'to', config.RedirectUrl);
        startLoadingProcess();
    }

    function startLoadingProcess() {
        const container = document.getElementById('loadingContainer');

        if (!container) {
            console.error('Loading container not found');
            setTimeout(() => window.location.href = config.RedirectUrl, 1000);
            return;
        }

        setTimeout(() => completeLoading(container), config.Duration);
    }

    function completeLoading(container) {
        container.classList.add('loading-complete');

        setTimeout(() => {
            window.location.href = config.RedirectUrl;
        }, 500);
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializeLoading);
    } else {
        initializeLoading();
    }
})();
