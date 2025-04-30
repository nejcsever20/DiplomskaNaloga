document.addEventListener("DOMContentLoaded", function () {
    // Info button logic
    document.querySelectorAll(".info-btn").forEach(function (btn) {
        btn.addEventListener("click", function () {
            const id = this.dataset.id; // Get the ID from the clicked button's data-id attribute
            const url = `https://localhost:7059/Skladi%C5%A1%C4%8Dnik/Index?handler=GetTransportInfo&id=${id}`; // Insert the dynamic id in the URL

            fetch(url)
                .then(res => res.json())
                .then(data => {
                    if (data.error) {
                        alert(data.error);
                        return;
                    }

                    // Update modal with transport data
                    document.getElementById("modal-stTransporta").textContent = data.stTransporta || "N/A";
                    document.getElementById("modal-registracija").textContent = data.registracija || "N/A";
                    document.getElementById("modal-voznik").textContent = data.voznik || "N/A";
                    document.getElementById("infoModal").style.display = "block";
                })
                .catch(err => {
                    alert("Failed to load transport info.");
                    console.error(err);
                });
        });
    });

    // Close modal logic
    document.querySelector(".close-btn").addEventListener("click", function () {
        document.getElementById("infoModal").style.display = "none";
    });

    // Close modal if clicked outside
    window.addEventListener("click", function (e) {
        const modal = document.getElementById("infoModal");
        if (e.target === modal) modal.style.display = "none";
    });

    // Print logic
    document.getElementById("print-btn").addEventListener("click", function () {
        const modalContent = document.querySelector(".modal-content");
        const printWindow = window.open("", "_blank", "width=800,height=600");

        const printHTML = `
            <!DOCTYPE html>
            <html>
            <head>
                <title>Transport Information</title>
                <style>
                    body { font-family: Arial, sans-serif; font-size: 14px; margin: 0; padding: 20px; }
                    .modal-content { padding: 20px; border: 1px solid #888; width: 80%; margin: 20px auto; background-color: #fefefe; }
                    .close-btn { display: none; }
                </style>
            </head>
            <body>
                ${modalContent.innerHTML}
            </body>
            </html>
        `;

        printWindow.document.write(printHTML);
        printWindow.document.close();

        printWindow.onload = function () {
            printWindow.focus();
            printWindow.print();
            printWindow.close();
        };
    });
});
