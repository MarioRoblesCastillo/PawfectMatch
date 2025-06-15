// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    // Handle pet details modal population
    const petDetailsModal = document.getElementById('petDetailsModal');
    if (petDetailsModal) {
        petDetailsModal.addEventListener('show.bs.modal', function (event) {
            // Button that triggered the modal
            const button = event.relatedTarget;

            // Extract info from data-bs-* attributes
            const petId = button.getAttribute('data-pet-id');
            const petName = button.getAttribute('data-pet-name');
            const petBreed = button.getAttribute('data-pet-breed');
            const petAge = button.getAttribute('data-pet-age');
            const petGender = button.getAttribute('data-pet-gender');

            // Update the modal's content
            const modalTitle = petDetailsModal.querySelector('.modal-title');
            const modalPetId = petDetailsModal.querySelector('#modalPetId');
            const modalPetName = petDetailsModal.querySelector('#modalPetName');
            const modalPetBreed = petDetailsModal.querySelector('#modalPetBreed');
            const modalPetAge = petDetailsModal.querySelector('#modalPetAge');
            const modalPetGender = petDetailsModal.querySelector('#modalPetGender');
            // const modalPetImage = petDetailsModal.querySelector('#modalPetImage'); // If you had unique images

            modalTitle.textContent = `Details for ${petName}`;
            modalPetId.textContent = petId;
            modalPetName.textContent = petName;
            modalPetBreed.textContent = petBreed;
            modalPetAge.textContent = petAge;
            modalPetGender.textContent = petGender;

            // If you had actual image URLs, you would update the src here:
            // modalPetImage.src = 'path/to/your/image.jpg';
            // For now, it keeps the SVG placeholder.
        });
    }

    // Handle adopt button click to show application form modal
    document.querySelectorAll('.adopt-btn').forEach(function (btn) {
        btn.addEventListener('click', function () {
            // Hide the pet details modal if open
            var petDetailsModalInstance = bootstrap.Modal.getInstance(document.getElementById('petDetailsModal'));
            if (petDetailsModalInstance) petDetailsModalInstance.hide();

            // Show the application form modal
            var applicationFormModal = new bootstrap.Modal(document.getElementById('applicationFormModal'));
            applicationFormModal.show();

     
        });
    });
});