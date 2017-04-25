# Change Log
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/) 
and this project adheres to [Semantic Versioning](http://semver.org/).

## [Unreleased]

### Added
* Added the ability to drag and drop objects into the node graph
* Added the `NodeGraphPolicy` class, which defines what operations a node
  graph can take
* Added Rotate, RotateAround, and Translate nodes
* Added Time nodes for getting delta times
* Added popup to member fields in nodes that display the display name of the
  member

### Changed
* Fuzzy search is now exhaustive, so it can return better results

### Fixed
* Fix dynamic node creation not working for overloaded methods
* Fix entry point caching error when trying to get the value of an unset field
* Fix incorrect uppercase check when calculating fuzzy search scores
* Fix NodeGraph editor losing reference to graph when entering play mode
* Fix input sockets still being editable when they are linked to

