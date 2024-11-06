var nameQuery = "";
var roleId = "";
var pageNumber = 1;
var totalPages = 1;
var isFetchingData = false;
$(document).ready(function () {
    fetchUsersResults();
});

function nextPage() {
    if (pageNumber < totalPages && !isFetchingData) {
        pageNumber++;
        fetchUsersResults();
    }
}

function prevPage() {
    if (pageNumber > 1 && !isFetchingData) {
        pageNumber--;
        fetchUsersResults();
    }
}

function search() {
    pageNumber = 1;
    fetchUsersResults();
}

function fetchUsersResults() {
    nameQuery = $('.nameQuery').val();
    isFetchingData = true;
    $.ajax({
        url: getListUrl,
        type: 'GET',
        data: {
            nameQuery: nameQuery,
            pageNumber: pageNumber
        },
        success: function (data) {
            generateUsersTable(data);
        },
        error: function (xhr, status, error) {
            console.error('Error fetching customer data:', error);
        },
        complete: function () {
            isFetchingData = false; // Reset the flag after data fetch is complete
        }
    });

}

function generateUsersTable(data) {
    // Update the pagination information
    pageNumber = data.pageNumber;
    totalPages = data.totalPages;

    // If current page out of range, then move current page to last range and retrieve data again
    if (pageNumber > totalPages) {
        pageNumber = totalPages;
        fetchUsersResults();
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
    if (data.userResults.length === 0) {
        $('.resultTableBody').append('<tr><td colspan="7"><p>No result found</p></td></tr>');
        return;
    }

    // Iterate over the user results and create table rows
    $.each(data.userResults, function (index, user) {
        const UpdatedAtFormatted = new Date(user.updatedAt).toLocaleString();
        const row = $('<tr>');
        row.append($('<td>').text(user.id));
        row.append($('<td>').text(user.username));
        row.append($('<td>').text(user.email));
        row.append($('<td>').text(UpdatedAtFormatted));
        if (user.role == 1) {
            row.append($('<td>').text('Admin'));
        } else if (user.role == 2) {
            row.append($('<td>').text('Người bán'));
        } else if (user.role == 3) {
            row.append($('<td>').text('Người mua'));
        } else {
            row.append($('<td>').text('N/A'));
        }
        if (user.status) {
            row.append($('<td>').text('Đã kích hoạt'));
        } else {
            row.append($('<td>').text('Chưa kích hoạt'));
        }
        const actionCell = $('<td>');
        const viewButton = $('<a>', {
            href: '/User/GetUser?userId=' + user.id,
            class: 'btn btn-sm btn-primary ps-2',
            html: '<i class="fas fa-eye"></i>'
        });
        actionCell.append($('<div>', { class: 'input-group flex-nowrap' })
            .append($('<div>', { class: 'ps-2' }).append(viewButton)));
        row.append(actionCell);

        $('.resultTableBody').append(row);
    });

}