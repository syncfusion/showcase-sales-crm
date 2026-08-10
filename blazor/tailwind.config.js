/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./Components/**/*.razor",
        "./Components/**/*.cshtml",
        "./Shared/**/*.razor",
        "./wwwroot/**/*.html",
    ],
    darkMode: ['selector', '[data-theme="dark"]'],
    theme: {
        extend: {},
    },
    corePlugins: {
        // Avoid clobbering Syncfusion component styles and Bootstrap resets
        preflight: false,
    },
    plugins: [],
}
