var nameQuery = "";
var roleId = "";
var statusId = "";
var totalPages = 1;
var isFetchingData = false;
var connection;

$(document).ready(function () {
    // Establish SignalR connection
    connection = new signalR.HubConnectionBuilder().withUrl("/categoryHub").build();

    // Define the event handler for ProductAdded
    connection.on("CategoryUpdated", function (category) {
        // Fetch and regenerate the table
        fetchCategoriesResults();
    });

    // Start the SignalR connection
    connection.start().catch(function (err) {
        return console.error(err.toString());
    });

    // Fetch the initial product results
    fetchCategoriesResults();
});

function nextPage() {
    if (pageNumber < totalPages && !isFetchingData) {
        pageNumber++;
        fetchCategoriesResults();
    }
}

function prevPage() {
    if (pageNumber > 1 && !isFetchingData) {
        pageNumber--;
        fetchCategoriesResults();
    }
}

function search() {
    pageNumber = 1;
    fetchCategoriesResults();
}

function fetchCategoriesResults() {
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
            generateCategoriesTable(data);
        },
        error: function (xhr, status, error) {
            console.error('Error fetching customer data:', error);
        },
        complete: function () {
            isFetchingData = false; // Reset the flag after data fetch is complete
        }
    });

}

function generateCategoriesTable(data) {
    // Update the pagination information
    pageNumber = data.pageNumber;
    totalPages = data.totalPages;

    // If current page out of range, then move current page to last range and retrieve data again
    if (pageNumber > totalPages) {
        pageNumber = totalPages;
        fetchCategoriesResults();
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
    if (data.categoryResults.length === 0) {
        $('.resultTableBody').append('<tr><td colspan="7"><p>No result found</p></td></tr>');
        return;
    }

    // Iterate over the category results and create table rows
    $.each(data.categoryResults, function (index, category) {
        const UpdatedAtFormatted = new Date(category.updatedAt).toLocaleString();
        const row = $('<tr>');
        row.append($('<td>').text(category.categoryName));
        row.append($('<td>').text(UpdatedAtFormatted));
        if (category.status) {
            row.append($('<td>').text('Đã kích hoạt'));
        } else {
            row.append($('<td>').text('Chưa kích hoạt'));
        }
        row.append($('<td>').text(category.description));
        
        const actionCell = $('<td>');
        const viewButton = $('<a>', {
            href: `/Category/GetCategory?categoryId=${category.id}&pageNumber=${pageNumber}`,
            class: 'btn btn-sm btn-primary ps-2',
            html: '<i class="fas fa-eye"></i>'
        });
        actionCell.append($('<div>', { class: 'input-group flex-nowrap' })
            .append($('<div>', { class: 'ps-2' }).append(viewButton)));
        row.append(actionCell);

        $('.resultTableBody').append(row);
    });
}
