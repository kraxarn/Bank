import 'package:flutter/material.dart';

//void main() =>
//	runApp(MyApp());

class MyApp extends StatelessWidget
{
	@override
	Widget build(BuildContext context)
	{
		return MaterialApp(
			title: "Example Design",
			theme: ThemeData.dark(),
			home:  ExpansionPanelsWidget()
		);
	}
}

class ListWidget extends StatefulWidget
{
	@override
	State createState() =>
		_ExpansionPanelsState();
}

/*
class ListState extends State<ListWidget>
{
	@override
	Widget build(BuildContext context)
	{
		return Scaffold(
			appBar: AppBar(
				title: Text("Example Design")
			),
			body: _buildList()
		);
	}
	
	Widget _buildList()
	{
		return ListView.builder(
			itemBuilder: (context, index) => EntryItem(data[index]),
			itemCount: data.length
		);
	}
}
*/

// -- New ExpansionPanel code -- //

typedef ItemPanelBodyBuilder<T> = Widget Function(ItemPanel<T> item);
typedef ValueToString<T>        = String Function(T value);

class EntryItemPanel extends StatelessWidget
{
	final String name;
	final String value;
	final String hint;
	final bool showHint;
	
	const EntryItemPanel(this.name, this.value, this.hint, this.showHint);
	
	Widget _crossFade(Widget first, Widget second, bool isExpanded)
	{
		return AnimatedCrossFade(
			firstChild:  first,
			secondChild: second,
			firstCurve:  const Interval(0.0, 0.6, curve: Curves.fastOutSlowIn),
			secondCurve: const Interval(0.4, 1.0, curve: Curves.fastOutSlowIn),
			sizeCurve: Curves.fastOutSlowIn,
			crossFadeState: isExpanded ? CrossFadeState.showSecond : CrossFadeState.showFirst,
			duration: const Duration(milliseconds: 200)
		);
	}
	
	@override
	Widget build(BuildContext context)
	{
		final theme     = Theme.of(context);
		final textTheme = theme.textTheme;
		
		return Row(
			children: <Widget>[
				Expanded(
					flex: 2,
					child: Container(
						margin: const EdgeInsets.only(left: 24.0),
						child: FittedBox(
							fit: BoxFit.scaleDown,
							alignment: Alignment.centerLeft,
							child: Text(
								name,
								style: textTheme.body1.copyWith(fontSize: 15.0)
							)
						)
					)
				),
				Expanded(
					flex: 3,
					child: Container(
						margin: const EdgeInsets.only(left: 24.0),
						child: _crossFade(
							Text(value, style: textTheme.caption.copyWith(fontSize: 15.0)),
							Text(hint,  style: textTheme.caption.copyWith(fontSize: 15.0)),
							showHint
						)
					)
				)
			]
		);
	}
}

class CollapsibleBody extends StatelessWidget
{
	final EdgeInsets margin;
	final Widget child;
	final VoidCallback onSave;
	final VoidCallback onCancel;
	
	const CollapsibleBody({this.margin = EdgeInsets.zero, this.child, this.onSave, this.onCancel});
	
	@override
	Widget build(BuildContext context)
	{
		final theme     = Theme.of(context);
		final textTheme = theme.textTheme;
		
		return Column(
			children: <Widget>[
				Container(
					margin: const EdgeInsets.only(
						left:   24.0,
						right:  24.0,
						bottom: 24.0
					) - margin,
					child: Container(
						child: DefaultTextStyle(
							style: textTheme.caption.copyWith(fontSize: 15.0),
							child: child
						)
					)
				),
				const Divider(height: 1.0),
				Container(
					padding: const EdgeInsets.symmetric(vertical: 16.0),
					child: Row(
						mainAxisAlignment: MainAxisAlignment.end,
						children: <Widget>[
							Container(
								margin: const EdgeInsets.only(right: 8.0),
								child: FlatButton(
									onPressed: onCancel,
									child: const Text(
										"CANCEL",
										style: TextStyle(
											// TODO: This doesn't look right on dark themes
											color: Colors.black54,
											fontSize: 15.0,
											fontWeight: FontWeight.w500
										)
									)
								)
							),
							Container(
								margin: const EdgeInsets.only(right: 8.0),
								child: FlatButton(
									onPressed: onSave,
									textTheme: ButtonTextTheme.accent,
									child: const Text("SAVE")
								)
							)
						]
					)
				)
			]
		);
	}
}

class ItemPanel<T>
{
	final String name;
	final String hint;
	final TextEditingController textEditingController;
	final ItemPanelBodyBuilder<T> builder;
	final ValueToString<T> valueToString;
	T value;
	var isExpanded = false;
	
	ItemPanel({
		this.name, this.value, this.hint, this.builder, this.valueToString
	}) : textEditingController = TextEditingController(text: valueToString(value));
	
	ExpansionPanelHeaderBuilder get headerBuilder =>
			(context, isExpanded) {
				return EntryItemPanel(name, valueToString(value), hint, isExpanded);
			};
	
	Widget build() =>
		builder(this);
}

class ExpansionPanelsWidget extends StatefulWidget
{
	@override
	State createState() =>
		_ExpansionPanelsState();
}

enum Location
{
	LocationA,
	LocationB,
	LocationC
}

// _ExpansionPanelsDemoState
class _ExpansionPanelsState extends State<ExpansionPanelsWidget>
{
	List<ItemPanel> _items;
	
	RadioListTile _radioTile(var value, String text, var groupValue, var onChanged)
	{
		return RadioListTile(
			value:      value,
			title:      Text(text),
			groupValue: groupValue,
			onChanged:  onChanged
		);
	}
	
	@override
	void initState()
	{
		super.initState();
		
		_items = <ItemPanel<dynamic>>[
			// Item 0: Preferences
			ItemPanel<String>(
				name: "Preferences",
				value: "Application Preferences",
				hint: "",
				valueToString: (value) => value,
				builder: (item) {
					void close() {
						setState(() {
						  item.isExpanded = false;
						});
					}
					return Form(
						child: Builder(
							builder: (context) {
								return CollapsibleBody(
									margin: const EdgeInsets.symmetric(horizontal: 16.0),
									onSave: () {
										Form.of(context).save();
										close();
									},
									onCancel: () {
										Form.of(context).reset();
										close();
									},
									child: Padding(
										padding: const EdgeInsets.symmetric(horizontal: 16.0),
										child: TextFormField(
											controller: item.textEditingController,
											decoration: InputDecoration(
												hintText: item.hint,
												labelText: item.name
											),
											onSaved: (value) {
												item.value = value;
											}
										)
									)
								);
							}
						)
					);
				}
			),
			// Item 1: Profile
			ItemPanel(
				name: "Location",
				value: Location.LocationA,
				hint: "Select Location",
				valueToString: (value) => value.toString().split('.')[1],
				builder: (item) {
					void close() {
						setState(() {
						  item.isExpanded = false;
						});
					}
					return Form(
						child: Builder(builder: (context) {
							return CollapsibleBody(
								onSave: () {
									Form.of(context).save();
									close();
								},
								onCancel: () {
									Form.of(context).reset();
									close();
								},
								child: FormField<Location>(
									initialValue: item.value,
									onSaved: (result) => item.value = result,
									builder: (field) {
										return Column(
											mainAxisSize: MainAxisSize.min,
											crossAxisAlignment: CrossAxisAlignment.start,
											children: <Widget>[
												_radioTile(Location.LocationA, "LocationA", field.value, field.didChange),
												_radioTile(Location.LocationB, "LocationB", field.value, field.didChange),
												_radioTile(Location.LocationC, "LocationC", field.value, field.didChange),
											]
										);
									}
								)
							);
						})
					);
				}
			),
			// Item 2: About
			ItemPanel(
				name:  "About",
				value: "v1.0-beta.1",
				hint:  "v1.0-beta.1",
				valueToString: (value) => "v1.0-beta.1",
				builder: (item) {
					void close() {
						setState(() {
							item.isExpanded = false;
						});
					}
					return Form(
						child: CollapsibleBody(
							onSave: () {
								Form.of(context).save();
								close();
							},
							onCancel: () {
								Form.of(context).reset();
								close();
							},
							child: Text(
								"Version 1.0 Beta 1",
								textAlign: TextAlign.start
							)
						)
					);
				}
			)
		];
	}
	
	@override
	Widget build(BuildContext context) =>
		Scaffold(
			appBar: AppBar(
				title: const Text("Settings")
			),
			body: SingleChildScrollView(
				child: SafeArea(
					child: Container(
						margin: const EdgeInsets.all(24.0),
						child: ExpansionPanelList(
							expansionCallback: (index, isExpanded) {
								setState(() {
								  _items[index].isExpanded = !isExpanded;
								});
							},
							children: _items.map<ExpansionPanel>((item) {
								return ExpansionPanel(
									isExpanded: item.isExpanded,
									headerBuilder: item.headerBuilder,
									body: item.build()
								);
							}).toList()
						)
					),
					top:    false,
					bottom: false
				)
			)
		);
}