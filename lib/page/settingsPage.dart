import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';

class ExpansionItem
{
	String expanded;
	String header;
	bool isExpanded = false;
	
	ExpansionItem(this.header, this.expanded);
}

List<ExpansionItem> getExpansionItems(length)
{
	return List.generate(length, (index) {
		return ExpansionItem(
			"Panel $index",
			"Item number $index"
		);
	});
}

class SettingsPage extends StatelessWidget
{
	final avatars = [
		"airplane",
		"boat",
		"burger",
		"car",
		"cat",
		"dog",
		"duck",
		"hat",
		"penguin",
		"phone",
		"shoe"
	];
	
	final themes = [
		"day",
		"night",
		"midnight"
	];
	
	final List<Tab> tabs = <Tab>[
		Tab(text: "LEFT"),
		Tab(text: "RIGHT")
	];
	
	_buildAvatars(context)
	{
		return avatars.map((avatar) {
			return _createAvatarTile(context, avatar);
		}).toList();
	}
	
	Widget _createAvatarTile(context, name)
	{
		return ListTile(
			leading: Image.asset("assets/avatar/$name.png"),
			title: Text(name.substring(0, 1).toUpperCase() + name.substring(1)),
			onTap: () => Navigator.pop(context, name)
		);
	}
	
	_showAvatarSelect(context)
	{
		showDialog<String>(
			context: context,
			builder: (context) {
				return SimpleDialog(
					title: Text("Select Avatar"),
					children: _buildAvatars(context)
				);
			}
		);
	}
	
	_showNotAddedYet(context)
	{
		showDialog<void>(
			context: context,
			builder: (context) {
				return AlertDialog(
					title: Text("Not Added"),
					content: Text("This has not been added yet, stay tuned!"),
					actions: <Widget>[
						FlatButton(
							child: Text("OK"),
							onPressed: () {
								Navigator.of(context).pop();
							},
						)
					],
				);
			}
		);
	}
	
	_createSwitch(String title, String subtitle)
	{
		return SwitchListTile(
			title: Text(title),
			subtitle: Text(subtitle),
			value: false,
			onChanged: (checked) { return checked; },
		);
	}
	
	_createAppTheme(String name)
	{
		return DropdownMenuItem<String>(
			value: name,
			child: Text(name.substring(0, 1).toUpperCase() + name.substring(1))
		);
	}
	
	@override
	Widget build(BuildContext context)
	{
		return Scaffold(
			appBar: AppBar(
				title: Text("Settings")
			),
			body: ListView(
				padding: EdgeInsets.all(16.0),
				children: <Widget>[
					// Application
					Card(
						child: Column(
							children: <Widget>[
								// Title
								ListTile(
									title: Text(
										"Application",
										style: Theme.of(context).textTheme.headline.copyWith(color: Colors.white)
									),
								),
								// Preferences
								/*
								ListTile(
									title: Text("Theme"),
									subtitle: Text("Dark"),
									onTap: () {
										showDialog<void>(
											context: context,
											builder: (context) {
												return AlertDialog(
													title: Text("Select Theme"),
													content: SingleChildScrollView(
														child: ListBody(
														children: <Widget>[
															ListTile(
																title: Text("Light"),
																onTap: () => Navigator.pop(context)
															),
															ListTile(
																title: Text("Dark"),
																onTap: () => Navigator.pop(context)
															),
															ListTile(
																title: Text("Black"),
																onTap: () => Navigator.pop(context)
															)
														],
													),
													),
													actions: <Widget>[
														FlatButton(
															child: Text("Cancel"),
															onPressed: () {}
														)
													],
												);
											}
										);
									}
								),
								 */
								// Test new dropdown theme
								ListTile(
									title: Text("Theme"),
									trailing: DropdownButton<String>(
										value: themes[1],
										onChanged: (value) {},
										items: themes.map<DropdownMenuItem<String>>((value) {
											return _createAppTheme(value);
										}).toList()
									)
								),
								_createSwitch(
									"Shorten Money",
									"Shorten and round money, for example, \$1,450 will show as \$1.5k"
								),
								_createSwitch(
									"Notification",
									"Display a notification when comeone transfers money to someone"
								),
								_createSwitch(
									"Prevent Sleep",
									"Prevent the device from going to sleep while on the game page"
								),
								_createSwitch(
									"Auto Roll Dice",
									"Automatically roll dice again and sum results if you roll double (up to 3 times)"
								),
								// Mostly a lazy way to add bottom padding
								ButtonBarTheme(
									child: ButtonBar(
										children: <Widget>[
										],
									),
								)
							],
						),
					),
					// Profile
					Card(
						child: Column(
							children: <Widget>
							[
								// Title
								ListTile(
									title: Text(
										"Profile",
										style: Theme.of(context).textTheme.headline.copyWith(color: Colors.white)
									),
								),
								
								// Avatar selection
								FlatButton(
									child: Image.asset(
										"assets/avatar/cat.png",
										height: 128,
									),
									onPressed: () { _showAvatarSelect(context); },
								),
								// Name entry
								Padding(
									padding: EdgeInsets.symmetric(
										horizontal: 16,
									),
									child: TextField(
										decoration: InputDecoration(
											labelText: "Name",
										),
									),
								),
								// Save button
								ButtonBarTheme(
									child: ButtonBar(
										children: <Widget>[
											FlatButton(
												child: Text("Save"),
												onPressed: () {},
											)
										],
									),
								)
							],
						),
					),
					// About
					Card(
						child: Column(
							children: <Widget>[
								// Title
								ListTile(
									title: Text(
										"About",
										style: Theme.of(context).textTheme.headline.copyWith(color: Colors.white)
									),
								),
								// Info
								ListTile(
									title: Text("Version"),
									subtitle: Text("v1.0-beta.14"),
								),
								// Changes
								ExpansionPanelList(
									expansionCallback: (index, expanded) {
										// ...
									},
									children: <ExpansionPanel>[
										ExpansionPanel(
											headerBuilder: (context, expanded) {
												return ListTile(
													title: Text("What's New")
												);
											},
											body: ListTile(
												title: Text("Please Wait...")
											)
										)
									]
								)
							],
						),
					)
				]
			)
		);
	}
}