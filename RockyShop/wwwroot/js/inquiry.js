let dataTable;

$(document).ready(() => {
    loadDataTable("GetAllInquires");
});

const loadDataTable = (url) => {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Inquiry/"+url
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "fullName", "width": "25%" },
            { "data": "email", "width": "25%" },
            { "data": "phoneNumber", "width": "25%" },
            {
                "data": "id",
                "render": (data) => {
                    return `
                        <div class="text-center">
                            <a href="/Inquiry/Details/${data}" class="btn btn-primary text-white" style="cursor:pointer">
                                <i class="fas fa-edit"></i>    
                            </a>
                        </div >`;
                },
                "width": "15%"
            }
        ]
    });
};