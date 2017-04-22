# Change Log
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/) 
and this project adheres to [Semantic Versioning](http://semver.org/).

## [Unreleased]

### Added
* Added the ability to drag and drop objects into the node graph
* Added the `NodeGraphPolicy` class, which defines what operations a node
  graph can take

### Changed
* Fuzzy search is now exhaustive, so it can return better results

### Fixed
* Fix dynamic node creation not working for overloaded methods
* Fix entry point caching error when trying to get the value of an unset field
* Fix incorrect uppercase check when calculating fuzzy search scores
* Fix NodeGraph editor losing reference to graph when entering play mode

