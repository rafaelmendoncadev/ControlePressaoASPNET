/**
 * ===== SISTEMA DE GRÁFICOS DE SAÚDE - CONTROLE PRESSÃO =====
 * 
 * Funcionalidades:
 * - Gráficos interativos com Chart.js
 * - Suporte a temas escuro/claro
 * - Filtros por período
 * - Responsividade completa
 * - Animações suaves
 * - Múltiplos tipos de gráfico
 */

class HealthCharts {
    constructor() {
        this.charts = {};
        this.currentTheme = this.getTheme();
        this.colors = this.getThemeColors();
        this.defaultOptions = this.getDefaultOptions();
        
        this.init();
    }

    /**
     * Inicializa o sistema de gráficos
     */
    init() {
        this.setupEventListeners();
        this.initializeCharts();
        console.log('📊 Sistema de gráficos carregado!');
    }

    /**
     * Obtém o tema atual
     */
    getTheme() {
        return document.documentElement.getAttribute('data-theme') || 'light';
    }

    /**
     * Obtém as cores baseadas no tema atual
     */
    getThemeColors() {
        const isDark = this.currentTheme === 'dark';
        
        return {
            primary: isDark ? '#8b5cf6' : '#6366f1',
            secondary: isDark ? '#a78bfa' : '#8b5cf6',
            success: isDark ? '#34d399' : '#10b981',
            danger: isDark ? '#f87171' : '#ef4444',
            warning: isDark ? '#fbbf24' : '#f59e0b',
            info: isDark ? '#60a5fa' : '#3b82f6',
            
            background: isDark ? '#1e293b' : '#ffffff',
            surface: isDark ? '#334155' : '#f8fafc',
            text: isDark ? '#f8fafc' : '#1e293b',
            textSecondary: isDark ? '#cbd5e1' : '#64748b',
            border: isDark ? '#475569' : '#e2e8f0',
            
            gradient: {
                primary: isDark 
                    ? ['#7c3aed', '#a78bfa'] 
                    : ['#6366f1', '#8b5cf6'],
                success: isDark 
                    ? ['#34d399', '#10b981'] 
                    : ['#10b981', '#059669'],
                danger: isDark 
                    ? ['#f87171', '#ef4444'] 
                    : ['#ef4444', '#dc2626'],
                warning: isDark 
                    ? ['#fbbf24', '#f59e0b'] 
                    : ['#f59e0b', '#d97706']
            }
        };
    }

    /**
     * Opções padrão para todos os gráficos
     */
    getDefaultOptions() {
        return {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: true,
                    position: 'top',
                    labels: {
                        usePointStyle: true,
                        padding: 20,
                        color: this.colors.text,
                        font: {
                            family: 'Inter, sans-serif',
                            size: 12,
                            weight: 500
                        }
                    }
                },
                tooltip: {
                    backgroundColor: this.colors.surface,
                    titleColor: this.colors.text,
                    bodyColor: this.colors.textSecondary,
                    borderColor: this.colors.border,
                    borderWidth: 1,
                    cornerRadius: 8,
                    displayColors: true,
                    font: {
                        family: 'Inter, sans-serif'
                    }
                }
            },
            scales: {
                x: {
                    grid: {
                        color: this.colors.border,
                        drawBorder: false
                    },
                    ticks: {
                        color: this.colors.textSecondary,
                        font: {
                            family: 'Inter, sans-serif',
                            size: 11
                        }
                    }
                },
                y: {
                    grid: {
                        color: this.colors.border,
                        drawBorder: false
                    },
                    ticks: {
                        color: this.colors.textSecondary,
                        font: {
                            family: 'Inter, sans-serif',
                            size: 11
                        }
                    }
                }
            },
            animation: {
                duration: 1000,
                easing: 'easeInOutQuart'
            },
            interaction: {
                intersect: false,
                mode: 'index'
            }
        };
    }

    /**
     * Inicializa todos os gráficos
     */
    initializeCharts() {
        // Verifica se temos dados disponíveis
        if (typeof window.chartData === 'undefined') {
            console.warn('Dados dos gráficos não encontrados. Carregando dados de exemplo...');
            window.chartData = this.getExampleData();
        }

        this.createPressureChart();
        this.createGlucoseChart();
        this.createComparisonChart();
        this.createHealthSummaryChart();
    }

    /**
     * Cria gráfico de tendência de pressão
     */
    createPressureChart() {
        const canvas = document.getElementById('pressureChart');
        if (!canvas) return;

        const ctx = canvas.getContext('2d');
        const data = window.chartData.pressure || [];
        
        if (this.charts.pressure) {
            this.charts.pressure.destroy();
        }

        this.charts.pressure = new Chart(ctx, {
            type: 'line',
            data: {
                labels: data.map(item => this.formatDate(item.date)),
                datasets: [
                    {
                        label: 'Sistólica',
                        data: data.map(item => item.systolic),
                        borderColor: this.colors.danger,
                        backgroundColor: this.colors.danger + '20',
                        fill: false,
                        tension: 0.4,
                        pointBackgroundColor: this.colors.danger,
                        pointBorderColor: this.colors.background,
                        pointBorderWidth: 2,
                        pointRadius: 5,
                        pointHoverRadius: 7
                    },
                    {
                        label: 'Diastólica',
                        data: data.map(item => item.diastolic),
                        borderColor: this.colors.primary,
                        backgroundColor: this.colors.primary + '20',
                        fill: false,
                        tension: 0.4,
                        pointBackgroundColor: this.colors.primary,
                        pointBorderColor: this.colors.background,
                        pointBorderWidth: 2,
                        pointRadius: 5,
                        pointHoverRadius: 7
                    }
                ]
            },
            options: {
                ...this.defaultOptions,
                scales: {
                    ...this.defaultOptions.scales,
                    y: {
                        ...this.defaultOptions.scales.y,
                        beginAtZero: false,
                        min: 60,
                        max: 200,
                        title: {
                            display: true,
                            text: 'Pressão (mmHg)',
                            color: this.colors.text,
                            font: {
                                family: 'Inter, sans-serif',
                                weight: 600
                            }
                        }
                    }
                },
                plugins: {
                    ...this.defaultOptions.plugins,
                    title: {
                        display: true,
                        text: 'Tendência da Pressão Arterial',
                        color: this.colors.text,
                        font: {
                            family: 'Inter, sans-serif',
                            size: 16,
                            weight: 600
                        },
                        padding: {
                            top: 10,
                            bottom: 30
                        }
                    }
                }
            }
        });
    }

    /**
     * Cria gráfico de tendência de glicose
     */
    createGlucoseChart() {
        const canvas = document.getElementById('glucoseChart');
        if (!canvas) return;

        const ctx = canvas.getContext('2d');
        const data = window.chartData.glucose || [];
        
        if (this.charts.glucose) {
            this.charts.glucose.destroy();
        }

        this.charts.glucose = new Chart(ctx, {
            type: 'line',
            data: {
                labels: data.map(item => this.formatDate(item.date)),
                datasets: [
                    {
                        label: 'Jejum',
                        data: data.filter(item => item.period === 'Jejum').map(item => item.value),
                        borderColor: this.colors.warning,
                        backgroundColor: this.createGradient(ctx, this.colors.gradient.warning),
                        fill: true,
                        tension: 0.4,
                        pointBackgroundColor: this.colors.warning,
                        pointBorderColor: this.colors.background,
                        pointBorderWidth: 2,
                        pointRadius: 4
                    },
                    {
                        label: 'Pós-refeição',
                        data: data.filter(item => item.period === 'PosRefeicao').map(item => item.value),
                        borderColor: this.colors.success,
                        backgroundColor: this.createGradient(ctx, this.colors.gradient.success),
                        fill: true,
                        tension: 0.4,
                        pointBackgroundColor: this.colors.success,
                        pointBorderColor: this.colors.background,
                        pointBorderWidth: 2,
                        pointRadius: 4
                    }
                ]
            },
            options: {
                ...this.defaultOptions,
                scales: {
                    ...this.defaultOptions.scales,
                    y: {
                        ...this.defaultOptions.scales.y,
                        beginAtZero: false,
                        min: 70,
                        max: 250,
                        title: {
                            display: true,
                            text: 'Glicose (mg/dL)',
                            color: this.colors.text,
                            font: {
                                family: 'Inter, sans-serif',
                                weight: 600
                            }
                        }
                    }
                },
                plugins: {
                    ...this.defaultOptions.plugins,
                    title: {
                        display: true,
                        text: 'Tendência da Glicose',
                        color: this.colors.text,
                        font: {
                            family: 'Inter, sans-serif',
                            size: 16,
                            weight: 600
                        },
                        padding: {
                            top: 10,
                            bottom: 30
                        }
                    }
                }
            }
        });
    }

    /**
     * Cria gráfico de comparação mensal
     */
    createComparisonChart() {
        const canvas = document.getElementById('comparisonChart');
        if (!canvas) return;

        const ctx = canvas.getContext('2d');
        const data = window.chartData.monthly || [];
        
        if (this.charts.comparison) {
            this.charts.comparison.destroy();
        }

        this.charts.comparison = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: data.map(item => item.month),
                datasets: [
                    {
                        label: 'Pressão Sistólica Média',
                        data: data.map(item => item.avgSystolic),
                        backgroundColor: this.createGradient(ctx, this.colors.gradient.danger),
                        borderColor: this.colors.danger,
                        borderWidth: 2,
                        borderRadius: 8,
                        borderSkipped: false,
                    },
                    {
                        label: 'Glicose Média',
                        data: data.map(item => item.avgGlucose),
                        backgroundColor: this.createGradient(ctx, this.colors.gradient.warning),
                        borderColor: this.colors.warning,
                        borderWidth: 2,
                        borderRadius: 8,
                        borderSkipped: false,
                        yAxisID: 'y1'
                    }
                ]
            },
            options: {
                ...this.defaultOptions,
                scales: {
                    ...this.defaultOptions.scales,
                    y: {
                        ...this.defaultOptions.scales.y,
                        type: 'linear',
                        display: true,
                        position: 'left',
                        title: {
                            display: true,
                            text: 'Pressão (mmHg)',
                            color: this.colors.text
                        }
                    },
                    y1: {
                        ...this.defaultOptions.scales.y,
                        type: 'linear',
                        display: true,
                        position: 'right',
                        title: {
                            display: true,
                            text: 'Glicose (mg/dL)',
                            color: this.colors.text
                        },
                        grid: {
                            drawOnChartArea: false,
                        },
                    }
                },
                plugins: {
                    ...this.defaultOptions.plugins,
                    title: {
                        display: true,
                        text: 'Comparação Mensal',
                        color: this.colors.text,
                        font: {
                            family: 'Inter, sans-serif',
                            size: 16,
                            weight: 600
                        }
                    }
                }
            }
        });
    }

    /**
     * Cria gráfico de resumo de saúde (donut)
     */
    createHealthSummaryChart() {
        const canvas = document.getElementById('healthSummaryChart');
        if (!canvas) return;

        const ctx = canvas.getContext('2d');
        const data = window.chartData.summary || {};
        
        if (this.charts.summary) {
            this.charts.summary.destroy();
        }

        this.charts.summary = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: ['Ótima', 'Normal', 'Limítrofe', 'Hipertensão'],
                datasets: [{
                    data: [
                        data.optimal || 0,
                        data.normal || 0,
                        data.borderline || 0,
                        data.hypertension || 0
                    ],
                    backgroundColor: [
                        this.colors.success,
                        this.colors.info,
                        this.colors.warning,
                        this.colors.danger
                    ],
                    borderColor: this.colors.background,
                    borderWidth: 3,
                    hoverBorderWidth: 5
                }]
            },
            options: {
                ...this.defaultOptions,
                cutout: '60%',
                plugins: {
                    ...this.defaultOptions.plugins,
                    title: {
                        display: true,
                        text: 'Distribuição das Medições',
                        color: this.colors.text,
                        font: {
                            family: 'Inter, sans-serif',
                            size: 16,
                            weight: 600
                        }
                    },
                    legend: {
                        ...this.defaultOptions.plugins.legend,
                        position: 'bottom'
                    }
                }
            }
        });
    }

    /**
     * Cria gradiente para gráficos
     */
    createGradient(ctx, colors) {
        const gradient = ctx.createLinearGradient(0, 0, 0, 400);
        gradient.addColorStop(0, colors[0] + '40');
        gradient.addColorStop(1, colors[1] + '10');
        return gradient;
    }

    /**
     * Formata data para exibição
     */
    formatDate(dateString) {
        const date = new Date(dateString);
        return date.toLocaleDateString('pt-BR', {
            day: '2-digit',
            month: '2-digit'
        });
    }

    /**
     * Dados de exemplo quando não há dados reais
     */
    getExampleData() {
        const dates = [];
        const today = new Date();
        
        for (let i = 30; i >= 0; i--) {
            const date = new Date(today);
            date.setDate(date.getDate() - i);
            dates.push(date.toISOString());
        }

        return {
            pressure: dates.map((date, index) => ({
                date: date,
                systolic: 120 + Math.random() * 40,
                diastolic: 80 + Math.random() * 20
            })),
            glucose: dates.map((date, index) => ({
                date: date,
                value: 90 + Math.random() * 60,
                period: Math.random() > 0.5 ? 'Jejum' : 'PosRefeicao'
            })),
            monthly: [
                { month: 'Jan', avgSystolic: 125, avgGlucose: 95 },
                { month: 'Fev', avgSystolic: 128, avgGlucose: 98 },
                { month: 'Mar', avgSystolic: 122, avgGlucose: 92 },
                { month: 'Abr', avgSystolic: 130, avgGlucose: 100 }
            ],
            summary: {
                optimal: 30,
                normal: 40,
                borderline: 20,
                hypertension: 10
            }
        };
    }

    /**
     * Atualiza tema dos gráficos
     */
    updateTheme() {
        this.currentTheme = this.getTheme();
        this.colors = this.getThemeColors();
        this.defaultOptions = this.getDefaultOptions();
        
        // Recria todos os gráficos com o novo tema
        this.initializeCharts();
    }

    /**
     * Aplica filtro por período
     */
    filterByPeriod(days) {
        const filteredData = this.filterData(days);
        window.chartData = filteredData;
        this.initializeCharts();
    }

    /**
     * Filtra dados por período
     */
    filterData(days) {
        const cutoffDate = new Date();
        cutoffDate.setDate(cutoffDate.getDate() - days);
        
        const originalData = window.originalChartData || window.chartData;
        
        return {
            pressure: originalData.pressure.filter(item => new Date(item.date) >= cutoffDate),
            glucose: originalData.glucose.filter(item => new Date(item.date) >= cutoffDate),
            monthly: originalData.monthly,
            summary: originalData.summary
        };
    }

    /**
     * Configura event listeners
     */
    setupEventListeners() {
        // Atualiza gráficos quando o tema muda
        document.addEventListener('click', (e) => {
            if (e.target.closest('#theme-toggle')) {
                setTimeout(() => this.updateTheme(), 300);
            }
        });

        // Filtros de período
        document.addEventListener('click', (e) => {
            if (e.target.closest('[data-filter]')) {
                const filter = e.target.closest('[data-filter]').dataset.filter;
                this.filterByPeriod(parseInt(filter));
                
                // Atualiza estado ativo dos botões
                document.querySelectorAll('[data-filter]').forEach(btn => 
                    btn.classList.remove('active'));
                e.target.closest('[data-filter]').classList.add('active');
            }
        });

        // Responsividade
        window.addEventListener('resize', () => {
            Object.values(this.charts).forEach(chart => {
                if (chart) chart.resize();
            });
        });
    }

    /**
     * Destrói todos os gráficos
     */
    destroy() {
        Object.values(this.charts).forEach(chart => {
            if (chart) chart.destroy();
        });
        this.charts = {};
    }
}

// Inicialização automática
document.addEventListener('DOMContentLoaded', () => {
    // Aguarda um pouco para garantir que todos os elementos estejam prontos
    setTimeout(() => {
        window.HealthCharts = new HealthCharts();
    }, 500);
});

// Utilitários para desenvolvedores
window.updateChartData = (data) => {
    if (window.HealthCharts) {
        window.originalChartData = window.chartData;
        window.chartData = data;
        window.HealthCharts.initializeCharts();
    }
};

window.filterCharts = (days) => {
    if (window.HealthCharts) {
        window.HealthCharts.filterByPeriod(days);
    }
}; 