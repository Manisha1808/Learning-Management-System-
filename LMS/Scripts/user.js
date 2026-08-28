$(document).ready(function () {

    debugger;

    $("#userGrid").jqGrid({
        url: "/Admin/Admin/GetUsers",
        datatype: "json",
        colNames: [
            "Name",
            "Email",
            "Role",
            "Course",
            "Enrollment Date",
            "Status",
            "Created Date",
            "Created By",
            "Actions"
        ],
        colModel: [
            {
                name: "Name",
                width: 100
            },
            {
                name: "Email",
                width: 100
            },
            {
                name: "Role",
                width: 100
            },
            {
                name: "Course",
                width: 100
            },
            {
                name: "EnrollmentDate",
                width: 150
            },
            {
                name: "Status",
                width: 100
            },
            {
                name: "CreatedDate",
                width: 100
            },
            {
                name: "CreatedBy",
                width: 100
            },
            {
                name: "Actions",
                width: 160,
                sortable: false,
                formatter: function (cellvalue, options, rowObject) {
                    return '<button type="button" ' +
                        'class="btn btn-primary btn-sm btn-edit" ' +
                        'data-id="' + rowObject.UserId + '">' +
                        'Edit</button> ' +

                        '<button type="button" ' +
                        'class="btn btn-danger btn-sm btn-delete" ' +
                        'data-id="' + rowObject.UserId + '">' +
                        'Delete</button>';
                }
            }
        ]
    });

    // EDIT
     $(document).on("click", ".btn-edit", function () {
        var userId = $(this).data("id");
        $.ajax({
            url: "/Admin/Admin/GetUserById",
            type: "GET",
            data: { id: userId },
            success: function (response) {
                if (response != null) {
                    $("#txtUserId").val(response.UserId);
                    $("#txtFirstName").val(response.FirstName);
                    $("#txtLastName").val(response.LastName);
                    $("#txtEmail").val(response.Email);
                    $("#txtRole").val(response.RoleId);
                    $("#userModal").modal("show");
                }
            },
            error: function () {
                alert("Error while loading user details.");
            }
        });
});

    // UPDATE
    $("#btnUpdateUser").click(function () {
        var userId = $("#txtUserId").val();
        var firstName = $("#txtFirstName").val();
        var lastName = $("#txtLastName").val();
        var email = $("#txtEmail").val();
        var roleText = $("#txtRole").val();
        var roleId;
        if (roleText == "Admin") {
            roleId = 1;
        }
        else if (roleText == "Employee") {
            roleId = 2;
        }
        $.ajax({
            url: "/Admin/Admin/UpdateUser",
            type: "POST",
            data: {
                UserId: userId,
                FirstName: firstName,
                LastName: lastName,
                Email: email,
                RoleId: roleId
            },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    $("#userModal").modal("hide");
        
                    $("#userGrid").jqGrid(
                        "setGridParam",
                        {
                            datatype: "json"
                        }
                    ).trigger("reloadGrid");
                }
                else {
                    alert("Update failed.");
                }

            },
            error: function () { alert("Error while updating user."); }
        });
    });

    // CLOSE
     $("#btnCloseUserModal").click(function () {
        $("#userModal").modal("hide");
    });

    // DELETE
    $(document).on("click", ".btn-delete", function () {
        var userId = $(this).data("id");
        var result = confirm( "Are you sure you want to delete this user?");
        if (result) {
            $.ajax({
                url: "/Admin/Admin/DeleteUser",
                type: "POST",
                data: { UserId: userId },
                success: function (response) {
                    if (response.success) {
                        alert(response.message);
                        $("#userGrid")
                            .setGridParam({
                                datatype: "json"
                            })
                            .trigger("reloadGrid");
                    }
                    else {
                        alert("Delete failed.");
                    }
                },
                error: function (xhr) {
                    console.log(xhr.responseText);
                    alert("Delete Error:\n" + xhr.responseText);
                }
            });
        }
    });
});

