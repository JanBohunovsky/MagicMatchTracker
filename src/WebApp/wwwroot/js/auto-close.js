let handlers = {};

export function subscribe(element, callbackObject) {
	const handler = (event) => {
		if (element.contains(event.target))
			return;

		callbackObject.invokeMethodAsync('OnClose')
			.catch(error => {
				console.error('An error has occured while invoking .NET `OnClose` method. Unsubscribing the handler associated with this object.\n', error);
				this.unsubscribe(element);
			});
	}

	document.addEventListener('click', handler);
	document.addEventListener('keyup', handler);
	handlers[element] = handler;
}

export function unsubscribe(element) {
	const handler = handlers[element];
	if (!handler) {
		console.error('Handler not found for element', element);
		return;
	}

	document.removeEventListener('click', handler);
	document.removeEventListener('keyup', handler);
	delete handlers[element];
}