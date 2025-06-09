document.addEventListener("DOMContentLoaded", function () {
    // Sample Static Data (You will later pass these from the View)
    var completedCount = 5;
    var preparedCount = 3;

    var totalIncome = 800;
    var totalExpenses = 200;

    // Initialize Requests Status Chart
    var ctxStatus = document.getElementById("statusChart").getContext('2d');
    var statusChart = new Chart(ctxStatus, {
        type: 'bar',
        data: {
            labels: ['Completed', 'Prepared'],
            datasets: [{
                label: 'Requests Count',
                data: [completedCount, preparedCount],
                backgroundColor: ['#4CAF50', '#FFC107'],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            scales: {
                y: { beginAtZero: true }
            }
        }
    });

    // Initialize Financial Summary Pie Chart
    var ctxFinance = document.getElementById("financeChart").getContext('2d');
    var financeChart = new Chart(ctxFinance, {
        type: 'pie',
        data: {
            labels: ['Income', 'Expenses'],
            datasets: [{
                data: [totalIncome, totalExpenses],
                backgroundColor: ['#36A2EB', '#FF6384'],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true
        }
    });

    // Handle Add Income/Expense
    document.getElementById('addTransactionButton').addEventListener('click', function () {
        var title = document.getElementById('transactionTitle').value;
        var amount = parseFloat(document.getElementById('transactionAmount').value);
        var type = document.getElementById('transactionType').value;

        if (!title || isNaN(amount)) {
            alert('Please enter valid title and amount.');
            return;
        }

        if (type === 'income') {
            totalIncome += amount;
        } else {
            totalExpenses += amount;
        }

        // Update Chart
        financeChart.data.datasets[0].data = [totalIncome, totalExpenses];
        financeChart.update();

        // Clear Form
        document.getElementById('transactionTitle').value = '';
        document.getElementById('transactionAmount').value = '';
        document.getElementById('transactionType').value = 'income';

        // Update Profit Display
        document.getElementById('profitDisplay').innerText = (totalIncome - totalExpenses).toFixed(2);
    });
});

function updateContractStatus(requestId, isSigned) {
    $.ajax({
        url: '/YourController/UpdateContractStatus',
        type: 'POST',
        data: { id: requestId, isSigned: isSigned },
        success: function () {
            alert('Contract status updated successfully.');
        },
        error: function () {
            alert('Error updating contract status.');
        }
    });
}

function openAttachmentModal(requestId) {
    $('#modalRequestId').val(requestId);

    // Load current attachment link (optional)
    $.get('/Admin/GetContractAttachment', { id: requestId }, function (data) {
        if (data.contractUrl) {
            $('#viewContractLink').attr('href', data.contractUrl).show();
        } else {
            $('#viewContractLink').hide();
        }
    });

    $('#attachmentModal').modal('show');
}

$(document).ready(function () {
    $('#attachmentForm').submit(function (e) {
        e.preventDefault();

        var formData = new FormData(this);
        $.ajax({
            url: '/Admin/UploadContract',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function () {
                showSuccessMessage();
                $('#attachmentModal').modal('hide');
            },
            error: function () {
                showErrorMessage();
            }



        });
    });
});

