document.addEventListener("DOMContentLoaded", function () {
    console.log("Redirect handler loaded");

    document.body.addEventListener("click", function (event) {
        const button = event.target.closest("button[data-redirect]");
        if (!button) return;

        const redirectUrl = button.getAttribute("data-redirect");
        const formId = button.getAttribute("data-form"); // passed from the button

        if (!redirectUrl || !formId) {
            console.warn("Missing redirect URL or form ID.");
            return;
        }

        const form = document.getElementById(formId);
        if (!form) {
            console.warn("Form not found:", formId);
            return;
        }

        console.log(`Injecting returnUrl and submitting form [${formId}]`);

        // Remove old redirect fields
        form.querySelectorAll('input[name="returnUrl"]').forEach(e => e.remove());

        // Inject hidden returnUrl input
        const hidden = document.createElement("input");
        hidden.type = "hidden";
        hidden.name = "returnUrl";
        hidden.value = redirectUrl;
        form.appendChild(hidden);

        form.submit();
    });
});
