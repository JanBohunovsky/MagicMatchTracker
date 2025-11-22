// Original source code from Blazored.FluentValidation and Steve Sanderson:
// https://github.com/Blazored/FluentValidation
// https://blog.stevensanderson.com/2019/09/04/blazor-fluentvalidation/

using System.Collections;
using Microsoft.AspNetCore.Components.Forms;

namespace MagicMatchTracker.Infrastructure.Validation;

/// <summary>
/// Converts between Blazor's <see cref="FieldIdentifier"/> and FluentValidation's PropertyPath.
/// </summary>
public sealed class FieldIdentifierConverter(EditContext editContext)
{
	private class Node
	{
		public Node? Parent { get; init; }
		public object? Model { get; init; }
		public string? PropertyName { get; init; }
		public int? Index { get; init; }
	}

	private static readonly char[] Separators = [',', '['];

	// ReSharper disable once CognitiveComplexity
	public FieldIdentifier ConvertFromPropertyPath(string propertyPath)
	{
		var model = editContext.Model;
		var nextTokenEnd = propertyPath.IndexOfAny(Separators);

		if (nextTokenEnd < 0)
			return new FieldIdentifier(model, propertyPath);

		var propertyNameSpan = propertyPath.AsSpan();

		while (true)
		{
			var nextToken = propertyNameSpan[..nextTokenEnd];
			propertyNameSpan = propertyNameSpan[(nextTokenEnd + 1)..];

			object? newModel;
			if (nextToken.EndsWith("]"))
			{
				// Indexer
				nextToken = nextToken[..^1];
				var property = model.GetType().GetProperty("Item");

				if (property is not null)
				{
					// We've got an Item property
					var indexerType = property.GetIndexParameters()[0].ParameterType;
					var indexerValue = Convert.ChangeType(nextToken.ToString(), indexerType);

					newModel = property.GetValue(model, [indexerValue]);
				}
				else
				{
					// If there's no Item property, try to cast the object to array.
					if (model is object[] array)
					{
						var indexerValue = int.Parse(nextToken);
						newModel = array[indexerValue];
					}
					else if (model is IReadOnlyList<object> list)
					{
						// Something about collection expressions in IReadOnlyList
						var indexerValue = int.Parse(nextToken);
						newModel = list[indexerValue];
					}
					else
					{
						throw new InvalidOperationException($"Could not find indexer on object of type {model.GetType().FullName}.");
					}
				}
			}
			else
			{
				// Property
				var property = model.GetType().GetProperty(nextToken.ToString());
				if (property is null)
					throw new InvalidOperationException($"Could not find property named {nextToken.ToString()} on object of type {model.GetType().FullName}.");

				newModel = property.GetValue(model);
			}

			if (newModel is null)
			{
				// Reached the end of nesting
				return new FieldIdentifier(model, nextToken.ToString());
			}

			model = newModel;

			nextTokenEnd = propertyPath.IndexOfAny(Separators);
			if (nextTokenEnd < 0)
				return new FieldIdentifier(model, propertyNameSpan.ToString());
		}
	}

	// ReSharper disable once CognitiveComplexity
	public string? ConvertToPropertyPath(FieldIdentifier fieldIdentifier)
	{
		var nodes = new Stack<Node>();
		nodes.Push(new Node
		{
			Model = editContext.Model,
		});

		while (nodes.Count > 0)
		{
			var currentNode = nodes.Pop();
			var currentModel = currentNode.Model;

			if (currentModel == fieldIdentifier.Model)
				return BuildPropertyPath(currentNode, fieldIdentifier);

			var nonPrimitiveProperties = currentModel?.GetType()
					.GetProperties()
					.Where(p => !p.PropertyType.IsPrimitive || p.PropertyType.IsArray)
				?? [];

			foreach (var nonPrimitiveProperty in nonPrimitiveProperties)
			{
				var instance = nonPrimitiveProperty.GetValue(currentModel);
				if (instance is null)
					continue;

				if (instance == fieldIdentifier.Model)
				{
					var node = new Node
					{
						Parent = currentNode,
						PropertyName = nonPrimitiveProperty.Name,
						Model = instance,
					};

					return BuildPropertyPath(node, fieldIdentifier);
				}

				if (instance is IEnumerable enumerable)
				{
					var itemIndex = 0;
					foreach (var item in enumerable)
					{
						nodes.Push(new Node
						{
							Parent = currentNode,
							PropertyName = nonPrimitiveProperty.Name,
							Index = itemIndex++,
							Model = item,
						});
					}

					continue;
				}

				nodes.Push(new Node
				{
					Parent = currentNode,
					PropertyName = nonPrimitiveProperty.Name,
					Model = instance,
				});
			}
		}

		return null;
	}

	private static string BuildPropertyPath(Node node, FieldIdentifier fieldIdentifier)
	{
		List<string> parts = [fieldIdentifier.FieldName];
		var currentNode = node;

		while (currentNode is not null)
		{
			if (!string.IsNullOrEmpty(currentNode.PropertyName))
			{
				if (currentNode.Index is not null)
				{
					parts.Add($"{currentNode.PropertyName}[{currentNode.Index}]");
				}
				else
				{
					parts.Add(currentNode.PropertyName);
				}
			}

			currentNode = currentNode.Parent;
		}

		return string.Join('.', parts.AsEnumerable().Reverse());
	}
}