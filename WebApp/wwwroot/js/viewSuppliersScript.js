var nameQuery = "";
var roleId = "";
var statusId = "";
var totalPages = 1;
var isFetchingData = false;
var connection;

$(document).ready(function () {
    // Establish SignalR connection
    connection = new signalR.HubConnectionBuilder().withUrl("/supplierHub").build();

    // Define the event handler for Supplier
    connection.on("SupplierUpdated", function (supplier) {
        // Fetch and regenerate the table
        fetchSuppliersResults();
    });

    // Start the SignalR connection
    connection.start().catch(function (err) {
        return console.error(err.toString());
    });
    fetchSuppliersResults();
});

function nextPage() {
    if (pageNumber < totalPages && !isFetchingData) {
        pageNumber++;
        fetchSuppliersResults();
    }
}

function prevPage() {
    if (pageNumber > 1 && !isFetchingData) {
        pageNumber--;
        fetchSuppliersResults();
    }
}

function search() {
    pageNumber = 1;
    fetchSuppliersResults();
}

function fetchSuppliersResults() {
    nameQuery = $('.nameQuery').val();
    statusId = $('.statusId').val();
    isFetchingData = true;
    $.ajax({
        url: getListUrl,
        type: 'GET',
        data: {
            nameQuery: nameQuery,
            statusId: statusId,
            pageNumber: pageNumber
        },
        success: function (data) {
            generateSuppliersTable(data);
        },
        error: function (xhr, status, error) {
            console.error('Error fetching supplier data:', error);
        },
        complete: function () {
            isFetchingData = false; // Reset the flag after data fetch is complete
        }
    });

}

function generateSuppliersTable(data) {
    // Update the pagination information
    pageNumber = data.pageNumber;
    totalPages = data.totalPages;

    // If current page out of range, then move current page to last range and retrieve data again
    if (pageNumber > totalPages) {
        pageNumber = totalPages;
        fetchSuppliersResults();
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
    if (data.supplierResults.length === 0) {
        $('.resultTableBody').append('<tr><td colspan="7"><p>No result found</p></td></tr>');
        return;
    }

    // Iterate over the user results and create table rows
    $.each(data.supplierResults, function (index, supplier) {
        const UpdatedAtFormatted = new Date(supplier.updatedAt).toLocaleString();
        const row = $('<tr>');
        row.append($('<td>').text(supplier.name));
        row.append($('<td>').text(supplier.address));
        row.append($('<td>').text(supplier.phone));
        row.append($('<td>').text(UpdatedAtFormatted));
        if (supplier.status) {
            row.append($('<td>').text('Đã kích hoạt'));
        } else {
            row.append($('<td>').text('Chưa kích hoạt'));
        }

        const actionCell = $('<td>');
        const viewButton = $('<a>', {
            href: `/Supplier/GetSupplier?supplierId=${supplier.id}&pageNumber=${pageNumber}`,
            class: 'btn btn-sm btn-primary ps-2',
            html: '<i class="fas fa-eye"></i>'
        });
        actionCell.append($('<div>', { class: 'input-group flex-nowrap' })
            .append($('<div>', { class: 'ps-2' }).append(viewButton)));
        row.append(actionCell);

        $('.resultTableBody').append(row);
    });

}