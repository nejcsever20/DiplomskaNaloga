document.addEventListener("click", (event) => {
    const el = event.target.closest("a, button, select");
    if (el) {
        fetch("/log-click", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                element: el.tagName,
                id: el.id || null,
                href: el.href || null,
                value: el.value || null,
                text: el.innerText || null
            })
        });
    }
});
