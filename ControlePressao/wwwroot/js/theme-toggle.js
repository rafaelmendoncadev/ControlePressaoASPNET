/**
 * ===== SISTEMA DE TOGGLE DE TEMA - CONTROLE PRESSÃO =====
 * 
 * Funcionalidades:
 * - Alternar entre tema claro e escuro
 * - Salvar preferência no localStorage
 * - Inicializar tema baseado na preferência salva ou sistema
 * - Animações suaves na transição
 * - Ícones dinâmicos no botão
 */

class ThemeToggle {
    constructor() {
        this.THEME_KEY = 'controlePressao-theme';
        this.THEMES = {
            LIGHT: 'light',
            DARK: 'dark'
        };
        
        this.currentTheme = this.getStoredTheme() || this.getSystemTheme();
        this.init();
    }

    /**
     * Inicializa o sistema de temas
     */
    init() {
        this.applyTheme(this.currentTheme);
        this.createToggleButton();
        this.setupEventListeners();
        this.animatePageLoad();
    }

    /**
     * Obtém o tema salvo no localStorage
     */
    getStoredTheme() {
        try {
            return localStorage.getItem(this.THEME_KEY);
        } catch (e) {
            console.warn('Erro ao acessar localStorage:', e);
            return null;
        }
    }

    /**
     * Obtém o tema preferido do sistema
     */
    getSystemTheme() {
        if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
            return this.THEMES.DARK;
        }
        return this.THEMES.LIGHT;
    }

    /**
     * Aplica o tema selecionado
     */
    applyTheme(theme) {
        const root = document.documentElement;
        
        // Remove tema anterior
        root.removeAttribute('data-theme');
        
        // Aplica novo tema
        if (theme === this.THEMES.DARK) {
            root.setAttribute('data-theme', 'dark');
        }
        
        this.currentTheme = theme;
        this.saveTheme(theme);
        this.updateToggleButton();
    }

    /**
     * Salva o tema no localStorage
     */
    saveTheme(theme) {
        try {
            localStorage.setItem(this.THEME_KEY, theme);
        } catch (e) {
            console.warn('Erro ao salvar tema:', e);
        }
    }

    /**
     * Alterna entre os temas
     */
    toggleTheme() {
        const newTheme = this.currentTheme === this.THEMES.LIGHT 
            ? this.THEMES.DARK 
            : this.THEMES.LIGHT;
        
        this.applyTheme(newTheme);
        this.animateToggle();
    }

    /**
     * Cria o botão de toggle do tema
     */
    createToggleButton() {
        // Verifica se o botão já existe
        if (document.getElementById('theme-toggle')) {
            return;
        }

        const button = document.createElement('button');
        button.id = 'theme-toggle';
        button.type = 'button';
        button.className = 'btn btn-outline-light ms-2';
        button.title = 'Alternar tema';
        button.innerHTML = this.getToggleButtonContent();
        
        // Estilização adicional
        button.style.cssText = `
            position: relative;
            overflow: hidden;
            width: 42px;
            height: 42px;
            border-radius: 50%;
            border: 2px solid rgba(255, 255, 255, 0.2);
            background: rgba(255, 255, 255, 0.1);
            backdrop-filter: blur(10px);
            transition: all 0.3s ease;
        `;

        // Adiciona o botão à navbar
        const navbar = document.querySelector('.navbar .navbar-nav:last-child');
        if (navbar) {
            const li = document.createElement('li');
            li.className = 'nav-item';
            li.appendChild(button);
            navbar.appendChild(li);
        }
    }

    /**
     * Obtém o conteúdo do botão baseado no tema atual
     */
    getToggleButtonContent() {
        const iconClass = this.currentTheme === this.THEMES.LIGHT 
            ? 'fas fa-moon' 
            : 'fas fa-sun';
        
        return `<i class="${iconClass}" style="font-size: 1.1rem;"></i>`;
    }

    /**
     * Atualiza o botão de toggle
     */
    updateToggleButton() {
        const button = document.getElementById('theme-toggle');
        if (button) {
            button.innerHTML = this.getToggleButtonContent();
            button.title = this.currentTheme === this.THEMES.LIGHT 
                ? 'Ativar tema escuro' 
                : 'Ativar tema claro';
        }
    }

    /**
     * Configura os event listeners
     */
    setupEventListeners() {
        // Toggle do tema
        document.addEventListener('click', (e) => {
            if (e.target.closest('#theme-toggle')) {
                e.preventDefault();
                this.toggleTheme();
            }
        });

        // Detecta mudanças na preferência do sistema
        if (window.matchMedia) {
            window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
                if (!this.getStoredTheme()) {
                    this.applyTheme(e.matches ? this.THEMES.DARK : this.THEMES.LIGHT);
                }
            });
        }

        // Keyboard shortcuts
        document.addEventListener('keydown', (e) => {
            // Ctrl + Shift + T para toggle do tema
            if (e.ctrlKey && e.shiftKey && e.key === 'T') {
                e.preventDefault();
                this.toggleTheme();
            }
        });
    }

    /**
     * Animação no carregamento da página
     */
    animatePageLoad() {
        // Adiciona classe de animação aos elementos principais
        setTimeout(() => {
            const elements = document.querySelectorAll('.card, .navbar, .btn, .alert');
            elements.forEach((el, index) => {
                el.style.animationDelay = `${index * 0.1}s`;
                el.classList.add('animate-fade-in-up');
            });
        }, 100);
    }

    /**
     * Animação durante o toggle
     */
    animateToggle() {
        const button = document.getElementById('theme-toggle');
        if (button) {
            // Efeito de rotação
            button.style.transform = 'rotate(360deg)';
            setTimeout(() => {
                button.style.transform = 'rotate(0deg)';
            }, 300);

            // Efeito de pulse
            button.style.animation = 'pulse 0.6s ease-in-out';
            setTimeout(() => {
                button.style.animation = '';
            }, 600);
        }

        // Efeito de transição suave no body
        document.body.style.transition = 'all 0.3s ease';
        setTimeout(() => {
            document.body.style.transition = '';
        }, 300);
    }

    /**
     * Obtém o tema atual
     */
    getCurrentTheme() {
        return this.currentTheme;
    }

    /**
     * Define um tema específico
     */
    setTheme(theme) {
        if (Object.values(this.THEMES).includes(theme)) {
            this.applyTheme(theme);
        }
    }
}

// Animações CSS adicionais
const additionalStyles = `
    @keyframes pulse {
        0% {
            transform: scale(1);
            box-shadow: 0 0 0 0 rgba(255, 255, 255, 0.4);
        }
        50% {
            transform: scale(1.05);
            box-shadow: 0 0 0 10px rgba(255, 255, 255, 0);
        }
        100% {
            transform: scale(1);
            box-shadow: 0 0 0 0 rgba(255, 255, 255, 0);
        }
    }

    #theme-toggle:hover {
        background: rgba(255, 255, 255, 0.2) !important;
        border-color: rgba(255, 255, 255, 0.4) !important;
        transform: scale(1.05);
    }

    #theme-toggle:active {
        transform: scale(0.95);
    }

    /* Animação de loading para elementos */
    .loading-shimmer {
        background: linear-gradient(90deg, 
            var(--bg-secondary) 25%, 
            var(--bg-tertiary) 50%, 
            var(--bg-secondary) 75%
        );
        background-size: 200% 100%;
        animation: shimmer 2s infinite;
    }

    @keyframes shimmer {
        0% {
            background-position: -200% 0;
        }
        100% {
            background-position: 200% 0;
        }
    }

    /* Transições suaves para elementos específicos */
    .navbar, .card, .btn, .form-control, .alert {
        transition: all 0.3s ease;
    }

    /* Hover effect para cards */
    .card:hover {
        transform: translateY(-4px);
    }

    /* Efeito de focus melhorado */
    .form-control:focus, .form-select:focus {
        transform: scale(1.02);
    }
`;

// Adiciona estilos adicionais
function addAdditionalStyles() {
    const style = document.createElement('style');
    style.textContent = additionalStyles;
    document.head.appendChild(style);
}

// Inicialização
document.addEventListener('DOMContentLoaded', () => {
    // Aplica tema imediatamente para evitar flash
    const themeToggle = new ThemeToggle();
    addAdditionalStyles();
    
    // Exporta para uso global
    window.ThemeToggle = themeToggle;
    
    console.log('🎨 Sistema de temas carregado!', {
        tema: themeToggle.getCurrentTheme(),
        atalho: 'Ctrl + Shift + T'
    });
});

// Utilitários para desenvolvedores
window.setTheme = (theme) => {
    if (window.ThemeToggle) {
        window.ThemeToggle.setTheme(theme);
    }
};

window.getTheme = () => {
    return window.ThemeToggle ? window.ThemeToggle.getCurrentTheme() : null;
}; 