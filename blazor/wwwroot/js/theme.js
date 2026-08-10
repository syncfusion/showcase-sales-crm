// Theme helper for the Sales CRM Blazor app.
window.salesCrmTheme = {
    apply: function (isDark) {
        const root = document.documentElement;
        root.dataset.theme = isDark ? 'dark' : 'light';
        if (document.body) {
            document.body.classList.toggle('e-dark-mode', !!isDark);
        }
    },
    getStored: function () {
        return window.localStorage.getItem('sales-crm-theme') || 'light';
    }
};
