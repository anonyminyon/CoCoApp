var nameQuery = "";
var roleId = "";
var totalPages = 1;
var isFetchingData = false;
var connection;
$(document).ready(function () {
    // Establish SignalR connection
    connection = new signalR.HubConnectionBuilder().withUrl("/orderHub").build();

    // Define the event handler for Order
    connection.on("OrderUpdated", function (order) {
        // Fetch and regenerate the table
        fetchOrderItemsResults();
    });
    // Start the SignalR connection
    connection.start().catch(function (err) {
        return console.error(err.toString());
    });
    fetchOrderItemsResults();
});

function nextPage() {
    if (pageNumber < totalPages && !isFetchingData) {
        pageNumber++;
        fetchOrderItemsResults();
    }
}

function prevPage() {
    if (pageNumber > 1 && !isFetchingData) {
        pageNumber--;
        fetchOrderItemsResults();
    }
}

function search() {
    pageNumber = 1;
    fetchOrderItemsResults();
}

function fetchOrderItemsResults() {
    nameQuery = $('.nameQuery').val();
    isFetchingData = true;
    $.ajax({
        url: getListUrl,
        type: 'GET',
        data: {
            nameQuery: nameQuery,
            pageNumber: pageNumber,
            orderId: orderId
        },
        success: function (data) {
            generateOrderItemsTable(data);
        },
        error: function (xhr, status, error) {
            console.error('Error fetching customer data:', error);
        },
        complete: function () {
            isFetchingData = false; // Reset the flag after data fetch is complete
        }
    });

}

function generateOrderItemsTable(data) {
    // Update the pagination information
    pageNumber = data.pageNumber;
    totalPages = data.totalPages;

    // If current page out of range, then move current page to last range and retrieve data again
    if (pageNumber > totalPages) {
        pageNumber = totalPages;
        fetchOrderItemsResults();
        return;
    }

    $('.pageNumber').html(pageNumber);
    $('.totalPages').html(totalPages);

    // Hide the previous page button if the current page is the first page
    if (pageNumber === 1 || pageNumber === 0) {
        $('.prevPage').hide();
    } else {
        $('.prevPage').show();
    }

    // Hide the next page button if the current page is the last page
    if (pageNumber === totalPages) {
        $('.nextPage').hide();
    } else {
        $('.nextPage').show();
    }

    // Clear the existing table body
    $('.resultTableBody').empty();

    // Check if there is no data
    if (data.orderResults.length === 0) {
        $('.resultTableBody').append('<tr><td colspan="7"><p>No result found</p></td></tr>');
        return;
    }

    // Iterate over the user results and create table rows
    $.each(data.orderResults, function (index, orderItem) {
        const row = $('<tr>');
        row.append($('<td>').text(orderItem.product.productName));
        row.append($('<td>').text(orderItem.volume));
        row.append($('<td>').text(orderItem.realVolume));
        row.append($('<td>').text(orderItem.productPrice.toLocaleString('vi-VN') + ' VNĐ'));
        row.append($('<td>').text(orderItem.total.toLocaleString('vi-VN') + ' VNĐ'));
        if (!orderItem.status) {
            row.append($('<td>').text('Chưa xác nhận'));
        } else {
            row.append($('<td>').text('Đã xác nhận'));
        }

        const actionCell = $('<td>');
        const viewButton = $('<a>', {
            href: `/Order/ViewOrderItemDetail?orderId=${orderItem.orderId}&productId=${orderItem.productId}&pageNumber=${pageNumber}`,
            class: 'btn btn-sm btn-primary ps-2',
            html: '<i class="fas fa-eye"></i>'
        });
        actionCell.append($('<div>', { class: 'input-group flex-nowrap' })
            .append($('<div>', { class: 'ps-2' }).append(viewButton)));
        row.append(actionCell);

        $('.resultTableBody').append(row);
    });

}