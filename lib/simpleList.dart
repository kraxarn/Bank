import 'package:flutter/material.dart';

// -- Example Code -- //

// One entry
class Entry
{
	final String title;
	final List<Entry> children;
	
	// Constructor with title and default value for children
	const Entry(this.title, [this.children = const <Entry>[]]);
}

// Data to display
const List<Entry> data = <Entry>[
	Entry(
		"Chapter A",
		<Entry>[
			Entry(
				"Section A0",
				<Entry>[
					Entry("Item A0.1"),
					Entry("Item A0.2"),
				],
			),
			Entry("Section A1"),
			Entry("Section A2"),
		],
	),
	Entry(
		"Chapter B",
		<Entry>[
			Entry("Section B0"),
			Entry("Section B1"),
		],
	),
];

// Displays one entry
class EntryItem extends StatelessWidget
{
	final Entry entry;
	
	const EntryItem(this.entry);
	
	Widget _buildTiles(Entry root)
	{
		if (root.children.isEmpty)
			return ListTile(
				title: Text(root.title),
			);
		return ExpansionTile(
			key: PageStorageKey<Entry>(root),
			title: Text(root.title),
			children: root.children.map(_buildTiles).toList()
		);
	}
	
	@override
	Widget build(BuildContext context) =>
		_buildTiles(entry);
}