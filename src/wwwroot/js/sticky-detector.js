let observers = {}

export function track(element, className) {
	const observer = new IntersectionObserver(
		([e]) => e.target.classList.toggle(className, e.intersectionRatio < 1),
		{ threshold: [1] });

	observer.observe(element);
	observers[element] = observer;
}

export function untrack(element) {
	const observer = observers[element];
	if (!observer) {
		console.error('Observer not found for element', element);
		return;
	}

	observer.disconnect();
	delete observers[element];
}