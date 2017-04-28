# Change Log
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/) 
and this project adheres to [Semantic Versioning](http://semver.org/).

## [Unreleased]

### Added
* Added the ability to drag and drop objects into the node graph
* Added the `NodeGraphPolicy` class, which defines what operations a node
  graph can take
* Added Rotate, RotateAround, Translate, MoveTo, If, While, Log, and Destroy
  nodes
* Added Time nodes for getting delta times
* Added popup to member fields in nodes that display the display name of the
  member
* Added Gate and If logic nodes
* Added tooltips on sockets with type, name, and description information
* Added a menu when no graph is loaded
* If the control key is down while clicking on a socket, all links to that
  socket are removed
* Search box can now be used by both keyboard and mouse
* Search box now has a scrollbar and can be scrolled by keyboard and mouse
* Search results are cached when the search string is made longer, which
  will reduce query time
* Factorio-style copy paste for member fields on a node

### Changed
* Fuzzy search is now exhaustive, so it can return better results
* Node boxes include the label to make it easier to drag the node
* Dynamic field and property nodes can now handle both set and get operations
* Dynamic reflection no longer includes special methods, items marked with the
  CompilerGeneratedAttribute, and any types that start with "<>", which are
  commonly anonymous types
* The width of nodes is now determined by the node instead of the sockets
* Various performance improvements for the NodeGraphPlayer and GraphEditor

### Fixed
* Fix dynamic node creation not working for overloaded methods
* Fix entry point caching error when trying to get the value of an unset field
* Fix incorrect uppercase check when calculating fuzzy search scores
* Fix NodeGraph editor losing reference to graph when entering play mode
* Fix input sockets still being editable when they are linked to
* Fix transparent box issue in Pro editor
* Fix dynamic ignore filters not working

