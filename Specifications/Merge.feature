Feature: Merge
	It happens when set of source properties produces single (or smaller result set) property
	{o1,o2,o3} => {o1`}, {o1,o2,o3} => {o1`,o2`} etc
	We may inherit holders or introduce new ones - it doesn't matter
	Technicall all source RRRs are archived and target produced. 
	The Merge operation may have as a result an origin property elimenated.

Background: there 2 properties for a different right-holders are exists.
			REGO is a pre defined type for Occupancy registration			
	Given Registration transaction "REGO" with No."TRN-PRE-OCCUPANCY-4-MERGE-01" is set current
	# LAUnit is not property - but RRR prototype
	And Current transaction has target LAUnit "LA-Occupancy-4-Merge-01"
	# Add property to LAUnit
	And Current transaction has property with Uid = "Property 4 (Merge 01)" in LAUnit "LA-Occupancy-4-Merge-01"
	# Map Party with LAUnit
	And Current transaction has party "Tim Taler" with role "Grantee" associated with LAUnit "LA-Occupancy-4-Merge-01"
	# and pre data for 2nd prop
	Given Registration transaction "REGO" with No."TRN-PRE-OCCUPANCY-4-MERGE-02" is set current
	And Current transaction has target LAUnit "LA-Occupancy-4-Merge-02"
	And Current transaction has property with Uid = "Property 4 (Merge 02)" in LAUnit "LA-Occupancy-4-Merge-02"
	And Current transaction has party "Steve Adams" with role "Grantee" associated with LAUnit "LA-Occupancy-4-Merge-02"	
	When transaction "TRN-PRE-OCCUPANCY-4-MERGE-01" is completed
	And transaction "TRN-PRE-OCCUPANCY-4-MERGE-02" is completed
@merger
Scenario: Simple merge of 2 properties
	This kinds operates with SU lifespan.
	It has 2 source properties and produce single target.
	At this example only single RRR type assumed

	Given Registration transaction "MERG" with No."TRN-MERGE-001" is set current
	# We have marked this LAUnit as target
	And Current transaction has target LAUnit "Simple Merge-LA-01"
	# Map property to LAUnit
	And Current transaction has property with Uid = "Property 5 Merge Result" in LAUnit "Simple Merge-LA-01"
	# Map party to LAUnit
	And Current transaction has party "Mark Twen (merger)" with role "Grantee" associated with LAUnit "Simple Merge-LA-01"	
	# we don't need for operational reasons any source LAUnits
	# but it makes simple to operate within the same Transaction instance having LAUnit properties collection
	And Current transaction has source LAUnit "Source 4 Simple Merge-LA-01"
	# Add neccessary properties to source container
	And Current transaction has property with Uid = "Property 4 (Merge 01)" in LAUnit "Simple Merge-LA-01"
	And Current transaction has property with Uid = "Property 4 (Merge 02)" in LAUnit "Simple Merge-LA-01"
	# We need to mark them as "to be cancelled"
	And Property "Property 4 (Merge 01)" in Transaction No"TRN-MERGE-001" Status set "Archived"
	And Property "Property 4 (Merge 02)" in Transaction No"TRN-MERGE-001" Status set "Archived"
	# Proceed with registration
	When transaction "TRN-MERGE-001" is completed
	Then Cancelled by "TRN-MERGE-001" 2 "Occupancy" rights
	And Party "Mark Twen (merger)" have active "Occupancy" rights on "Property 5 Merge Result"
	And Property "Property 4 (Merge 01)" is archived
	And Property "Property 4 (Merge 02)" is archived

Scenario: Amalgamation merge of 2 properties
	This kinds operates with SU lifespan.
	It has 2 source properties and produce single target. Target is one of the source
	At this example only single RRR type assumed
	
	# Preparation

	#precondition part reg 2 prop
	Given Registration transaction "REGO" with No."TRN-PRE-OCCUPANCY-4-MERGE-03" is set current
	And Current transaction has target LAUnit "LA-Occupancy-4-Merge-03"
	And Current transaction has property with Uid = "Property 7 (A-Merge 01)" in LAUnit "LA-Occupancy-4-Merge-03"
	And Current transaction has party "Robert Taler" with role "Grantee" associated with LAUnit "LA-Occupancy-4-Merge-03"
	#and pre data for 2nd prop
	Given Registration transaction "REGO" with No."TRN-PRE-OCCUPANCY-4-MERGE-04" is set current
	And Current transaction has target LAUnit "LA-Occupancy-4-Merge-04"
	And Current transaction has property with Uid = "Property 7 (A-Merge 02)" in LAUnit "LA-Occupancy-4-Merge-04"
	And Current transaction has party "John Adams" with role "Grantee" associated with LAUnit "LA-Occupancy-4-Merge-04"	
	When transaction "TRN-PRE-OCCUPANCY-4-MERGE-03" is completed
	And transaction "TRN-PRE-OCCUPANCY-4-MERGE-04" is completed

	# Main part
	Given Registration transaction "MERG" with No."TRN-A-MERGE-002" is set current
	And Current transaction has target LAUnit "LA-Occupancy-4-AmalgaMerge-01"
	#lets assume 1st property will be put to target
	And Current transaction has property with Uid = "Property 7 (A-Merge 01)" in LAUnit "LA-Occupancy-4-AmalgaMerge-01"
	And Current transaction has party "Matt Tomson (merger)" with role "Grantee" associated with LAUnit "LA-Occupancy-4-AmalgaMerge-01"	
	# we don't need for operational reasons source LAUnit, but it makes simple to operate within the same Transaction instance having LAUnit properties collection 
	And Current transaction has source LAUnit "LA-Src-4-AmalgaMerge-01"
	And Current transaction has property with Uid = "Property 7 (A-Merge 01)" in LAUnit "LA-Src-4-AmalgaMerge-01"
	And Current transaction has property with Uid = "Property 7 (A-Merge 02)" in LAUnit "LA-Src-4-AmalgaMerge-01"
	# in this case we chosen 1st to survive
	And Property "Property 7 (A-Merge 02)" in Transaction No"TRN-A-MERGE-002" Status set "Archived"
	When transaction "TRN-A-MERGE-002" is completed
	Then Cancelled by "TRN-A-MERGE-002" 2 "Occupancy" rights
	And Party "Matt Tomson (merger)" have active "Occupancy" rights on "Property 7 (A-Merge 01)"
	And Property "Property 7 (A-Merge 01)" is not archived
	And Property "Property 7 (A-Merge 02)" is archived
