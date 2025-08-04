// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Máscara para data brasileira (dd/mm/aaaa)
function applyDateMask(input) {
    input.addEventListener('input', function(e) {
        let value = e.target.value.replace(/\D/g, '');
        
        if (value.length >= 2) {
            value = value.substring(0, 2) + '/' + value.substring(2);
        }
        if (value.length >= 5) {
            value = value.substring(0, 5) + '/' + value.substring(5, 9);
        }
        
        e.target.value = value;
    });
}

// Aplicar máscara quando o documento carregar
document.addEventListener('DOMContentLoaded', function() {
    const dateInput = document.querySelector('input[name="DataNascimento"]');
    if (dateInput) {
        applyDateMask(dateInput);
    }
});

// Write your JavaScript code.
