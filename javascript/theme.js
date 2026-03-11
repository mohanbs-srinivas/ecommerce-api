/**
 * Theme utility for light/dark mode support.
 * Reads, applies, and persists the user's theme preference.
 * Falls back to the OS-level prefers-color-scheme on first visit.
 */

const THEME_KEY = 'theme-preference';
const DARK_THEME = 'dark';
const LIGHT_THEME = 'light';

/**
 * Returns the stored theme preference, or detects from system preference.
 * @returns {'light' | 'dark'}
 */
function getPreferredTheme() {
    const stored = localStorage.getItem(THEME_KEY);
    if (stored === DARK_THEME || stored === LIGHT_THEME) {
        return stored;
    }
    return window.matchMedia('(prefers-color-scheme: dark)').matches
        ? DARK_THEME
        : LIGHT_THEME;
}

/**
 * Applies the given theme by setting the data-theme attribute on <html>.
 * @param {'light' | 'dark'} theme
 */
function applyTheme(theme) {
    document.documentElement.setAttribute('data-theme', theme);
    const toggleBtn = document.getElementById('theme-toggle');
    if (toggleBtn) {
        const isDark = theme === DARK_THEME;
        toggleBtn.setAttribute('aria-label', isDark ? 'Switch to light mode' : 'Switch to dark mode');
        toggleBtn.setAttribute('aria-pressed', String(isDark));
        toggleBtn.textContent = isDark ? '☀️' : '🌙';
    }
}

/**
 * Persists the theme preference to localStorage.
 * @param {'light' | 'dark'} theme
 */
function saveTheme(theme) {
    localStorage.setItem(THEME_KEY, theme);
}

/**
 * Toggles between light and dark mode, then saves and applies.
 */
function toggleTheme() {
    const current = document.documentElement.getAttribute('data-theme') || getPreferredTheme();
    const next = current === DARK_THEME ? LIGHT_THEME : DARK_THEME;
    applyTheme(next);
    saveTheme(next);
}

/**
 * Initializes the theme on page load. Call as early as possible to avoid FOUC.
 */
function initTheme() {
    applyTheme(getPreferredTheme());

    // Keep in sync with system preference changes when no stored preference exists
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
        if (!localStorage.getItem(THEME_KEY)) {
            applyTheme(e.matches ? DARK_THEME : LIGHT_THEME);
        }
    });
}

// Initialize as soon as this script loads (avoids flash of unstyled content)
initTheme();
