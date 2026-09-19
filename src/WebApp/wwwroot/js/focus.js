export function focusFirst(parent, selector) {
	const element = (parent || document).querySelector(selector);
	if (element) {
		element.focus();
	}
}