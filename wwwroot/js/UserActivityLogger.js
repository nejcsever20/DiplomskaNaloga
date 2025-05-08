<script>
    document.addEventListener("DOMContentLoaded", function() {
        // Track all button clicks
        const buttons = document.querySelectorAll("button, a");

        buttons.forEach(button => {
        button.addEventListener("click", function () {
            logUserActivity(`Clicked: ${button.textContent || button.getAttribute('href')}`, "Button Click");
        });
        });

    // Track all form submissions
    const forms = document.querySelectorAll("form");

        forms.forEach(form => {
        form.addEventListener("submit", function () {
            logUserActivity(`Submitted: ${form.getAttribute('action')}`, "Form Submission");
        });
        });

    function logUserActivity(action, type) {
        fetch("/api/logUserActivity", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ action: action, type: type })
        });
        }
    });
</script>
