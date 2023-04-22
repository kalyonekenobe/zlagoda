let modelsToPrint = ['employees', 'categories', 'clients', 'products'];

modelsToPrint.forEach(modelName => {
    let printButton = document.getElementById(`print-${modelName}`);
    if (printButton) {
        printButton.onclick = event => {
            let table = document.getElementById(`${modelName}-table`);
            if (table) {
                window.print();
            }
        }
    }
});

let closeErrorButtons = document.querySelectorAll(".error .close");
closeErrorButtons.forEach(button => button.onclick = event => {
    let parent = button.parentElement;
    parent.style.display = "none";
});