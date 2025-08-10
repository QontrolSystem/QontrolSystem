document.addEventListener("DOMContentLoaded", function () {
    const ctx = document.getElementById("ticketStatusChart").getContext("2d");

    const chartData = window.ticketStatusCounts || {
        Open: 0,
        InProgress: 0,
        Resolved: 0,
        Closed: 0
    };

    const ticketChart = new Chart(ctx, {
        type: "doughnut",
        data: {
            labels: Object.keys(chartData),
            datasets: [{
                data: Object.values(chartData),
                backgroundColor: [
                    "#f44336", // Open
                    "#ff9800", // In Progress
                    "#4caf50", // Resolved
                    "#2196f3"  // Closed
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: "bottom"
                },
                title: {
                    display: true,
                    text: "Ticket Status Overview"
                }
            }
        }
    });
});