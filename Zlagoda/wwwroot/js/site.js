let models = ['employees', 'categories', 'clients', 'products', 'store-products'];

models.forEach(modelName => {
    let printButton = document.getElementById(`print-${modelName}`);
    if (printButton) {
        printButton.onclick = event => {
            let table = document.getElementById(`${modelName}-table`);
            if (table) {
                window.print();
            }
        }
    }

    let findModelDetailsInput = document.getElementById(`get-${modelName}-info-input`);
    let button = document.getElementById(`get-${modelName}-info-btn`);
    if (findModelDetailsInput && button) {
        findModelDetailsInput.oninput = event => {
            let value = event.target.value;
            button.setAttribute('href', `brief-info/${value}`);
        }
    }
});

let closeErrorButtons = document.querySelectorAll(".error .close");
closeErrorButtons.forEach(button => button.onclick = event => {
    let parent = button.parentElement;
    parent.style.display = "none";
});

let storeProductTypeSelector = document.querySelector('.store-product-type-selector');
if (storeProductTypeSelector) {
    let buttons = storeProductTypeSelector.querySelectorAll('span');
    let containers = document.querySelectorAll('.store-product-type-container');
    buttons.forEach(button => button.onclick = event => {
        buttons.forEach(button => button.classList.remove('selected'));
        button.classList.add('selected');
        containers.forEach(container => {
            if (container.dataset.type === button.dataset.type) {
                container.classList.add('visible');
            } else {
                container.classList.remove('visible');
            }
        })
    });
}