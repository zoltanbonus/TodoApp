// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$('#deleteTodoModal').on('show.bs.modal', function (e) {
    var todoId = $(e.relatedTarget).data('todo-id');
    $(e.currentTarget).find('input[name="id"]').val(todoId);
});

function onEditButtonClick(id) {
    toggleTodoButtons(id);
    setButtonsDisabledAttribute(true);
}

function onSaveButtonClick(id) {
    var description = $(`input[data-todo-id="${id}"`).val();
    $.post("/Home/UpdateTodo", { id, description }, function (response) {
        if (response == "ERROR") {
            alert("error");
        } else {
            $(`label[data-todo-id="${id}"`).text(description);
            revertButtonState(id);
        }
    })
}

function onCancelEditButtonClick(id) {
    revertButtonState(id);
    var originalDescription = $(`label[data-todo-id="${id}"`).text();
    $(`input[name="edit-todo-description-${id}"`).val(originalDescription);
}

function revertButtonState(id) {
    toggleTodoButtons(id);
    setButtonsDisabledAttribute(false);
}

function toggleTodoButtons(id) {
    $(`input[data-todo-id="${id}"`).toggle();
    $(`label[data-todo-id="${id}"`).toggle();

    $(`button[name="cancel-edit-button-${id}"`).toggle();
    $(`button[name="delete-button-${id}"`).toggle();    

    $(`button[name="edit-button-${id}"`).toggle();
    $(`button[name="save-button-${id}"`).toggle();
}

function setButtonsDisabledAttribute(newValue) {
    $("button[name^='edit-button']").attr("disabled", newValue);
    $("button[name^='delete-button']").attr("disabled", newValue);
    $("button[name^='complete-button']").attr("disabled", newValue);

}