import 'package:flutter/material.dart';

class MainPage extends StatelessWidget
{
	_openCreateDialog(context)
	{
		/*
		showModalBottomSheet(
			context: context,
			builder: (context) {
				return Padding(
					padding: EdgeInsets.all(16.0),
					child: Table(
						
							/*
							,
							,
							Row(
								children: <Widget>[
								
								],
							),
							 */
						],
					)
				);
			}
		);
		 */
		
		showDialog<void>(
			context: context,
			builder: (context) {
				return AlertDialog(
					title: Text("Create Game"),
					content: Row(
						crossAxisAlignment: CrossAxisAlignment.end,
						children: <Widget>[
							Expanded(
								child: Padding(
									padding: EdgeInsets.only(
										bottom: 8.0
									),
									child: TextField(
										keyboardType: TextInputType.numberWithOptions(
											signed: true,
											decimal: true
										),
										decoration: InputDecoration(
											labelText: "Starting Money"
										)
									)
								)
							),
							DropdownButton<String>(
								value: "K",
								onChanged: (value) {},
								items: [" ", "K", "M"].map((value) {
									return DropdownMenuItem<String>(
										value: value,
										child: Text(value)
									);
								}).toList()
							),
						],
						/*
						child: ListTile(
							title: TextField(
								keyboardType: TextInputType.numberWithOptions(
									signed: true,
									decimal: true
								),
								decoration: InputDecoration(
									labelText: "Starting Money"
								)
							),
							trailing: DropdownButton<String>(
								value: "K",
								onChanged: (value) {},
								items: [" ", "K", "M"].map((value) {
									return DropdownMenuItem<String>(
										value: value,
										child: Text(value)
									);
								}).toList()
							)
						)
						 */
					),
					/*
					content: Table(
						children: [
							// Title (not needed if dialog
							TableRow(
								children: [
									Text(
										"Create New Game",
										style: Theme.of(context).textTheme.headline
									)
								]
							),
							// Spacing
							TableRow(
								children: [
									SizedBox(
										height: 16.0,
									)
								]
							),
							// Money entry
							TableRow(
								children: [
									TextField(
										keyboardType: TextInputType.numberWithOptions(
											signed: true,
											decimal: true
										),
										decoration: InputDecoration(
											labelText: "Starting Money",
											border: OutlineInputBorder(
												borderRadius: BorderRadius.all(Radius.circular(8.0))
											)
										)
									)
								]
							)
						]
					),
					 */
					actions: <Widget>[
						FlatButton(
							child: Text("OK"),
							onPressed: () {
								Navigator.of(context).pop();
							},
						)
					]
				);
			}
		);
	}
	
	@override
	Widget build(BuildContext context)
	{
		return Scaffold(
			appBar: AppBar(
				title: Text("Virtual Bank"),
				actions: <Widget>[
					IconButton(
						icon: Icon(Icons.search),
						onPressed: () { }
					),
					IconButton(
						icon: Icon(Icons.settings),
						onPressed: () {
							Navigator.of(context).pushNamed("/settings");
						}
					)
				]
			),
			floatingActionButton: FloatingActionButton(
				child: Icon(Icons.add),
				onPressed: () {
					Navigator.of(context).pushNamed("/create");
				},
			),
			body: Padding(
				padding: EdgeInsets.all(16),
				child: Column(
					children: <Widget>[
						// Avatar and name row
						Row(
							mainAxisSize: MainAxisSize.min,
							children: <Widget>[
								Image.asset("assets/avatar/cat.png", height: 128),
								// Two rows for title and subtitle
								Padding(
									padding: EdgeInsets.only(
										left: 16
									),
									child: Column(
										children: <Widget>[
											Text(
												"Welcome",
												style: Theme.of(context).textTheme.headline.copyWith(color: Colors.white)
											),
											Text("OnePlus 7 Pro")
										],
									),
								)
							],
						)
					]
				)
			)
		);
	}
}