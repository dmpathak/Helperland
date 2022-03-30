// for left-right side manu 
document.getElementById("expand_btn").addEventListener('click', () => {
    document.getElementById("expand_btn_container").classList.toggle("expand-btn-container-open")

})
document.getElementById("expand_btn_res").addEventListener('click', () => {
    document.getElementById("expand_btn_container").classList.toggle("expand-btn-container-open")

})

// for popover 
var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'))
var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
    return new bootstrap.Popover(popoverTriggerEl, { sanitize: false })
})


//particular user ni upcoming and complete service
window.addEventListener("load", () => {
	fetch("/Provider/GetEvents", { method: "GET" })
		.then((res) => res.json())
		.then((data) => {
			const calendarElement = document.getElementById("Calender");
			const calendar = new FullCalendar.Calendar(calendarElement, {
				eventDisplay: "block",
				displayEventTime: false,
				displayEventEnd: false,
				customButtons: {
					completedBtn: {
						text: "Completed",
					},
					upcomingBtn: {
						text: "Upcoming",
					},
				},
				headerToolbar: {
					start: "prev,next title",
					center: "",
					end: "completedBtn upcomingBtn",
				},
				initialView: "dayGridMonth",
				events: data,
				eventClick: (info) => {
					openDetailsModal(parseInt(info.event._def.publicId));
				},
			});
			calendar.render();
		})
		.catch((err) => console.log(err.message));
});