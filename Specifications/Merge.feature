Feature: Merge
	It happens when set of source properties produces single (or smaller result set) property
	{o1,o2,o3} => {o1`}, {o1,o2,o3} => {o1`,o2`} etc
	We may inherit holders or introduce new ones - it doesn't matter
	Technicall all source RRRs are archived and target produced. 
	The Merge operation may have as a result an origin property elimenated.

Background: there 2 properties for a different right-holders are exists.
	Given Registration transaction "REGO" with No."TRN-OCCUPANCY-4-MERGE-001" is set current
	And Current transaction has target LAUnit "Occupancy4Merge-LA-01"
	And Current transaction has property with Uid = "Property 4 Merge 01" in LAUnit "Occupancy4Merge-LA-01"
	And Current transaction has party "Tim Taler" with role "Grantee" associated with LAUnit "Occupancy4Merge-LA-01"
	#and pre data for 2nd prop
	Given Registration transaction "REGO" with No."TRN-OCCUPANCY-4-MERGE-002" is set current
	#And Registration transaction "REGO" with No."TRN-OCCUPANCY-4-MERGE-002" is set current
	And Current transaction has target LAUnit "Occupancy4Merge-LA-02"
	And Current transaction has property with Uid = "Property 4 Merge 02" in LAUnit "Occupancy4Merge-LA-02"
	And Current transaction has party "Steve Adams" with role "Grantee" associated with LAUnit "Occupancy4Merge-LA-02"	
	When transaction "TRN-OCCUPANCY-4-MERGE-001" is completed
	And transaction "TRN-OCCUPANCY-4-MERGE-002" is completed
@merger
Scenario: Simple merge of 2 properties
	This kinds operates with SU lifespan.
	It has 2 source properties and produce single target.
	At this example only single RRR type assumed

	Given Registration transaction "MERG" with No."TRN-MERGE-001" is set current
	And Current transaction has target LAUnit "Simple Merge-LA-01"
	And Current transaction has property with Uid = "Property 5 Merge Result" in LAUnit "Simple Merge-LA-01"
	And Current transaction has party "Mark Twen (merger)" with role "Grantee" associated with LAUnit "Simple Merge-LA-01"	
	# we don't need for operational reasons source LAUnit, but it makes simple to operate within the same Transaction instance having LAUnit properties collection 
	And Current transaction has source LAUnit "Source 4 Simple Merge-LA-01"
	And Current transaction has property with Uid = "Property 4 Merge 01" in LAUnit "Simple Merge-LA-01"
	And Current transaction has property with Uid = "Property 4 Merge 02" in LAUnit "Simple Merge-LA-01"
	And Property "Property 4 Merge 01" in Transaction No"TRN-MERGE-001" Status set "Archived"
	And Property "Property 4 Merge 02" in Transaction No"TRN-MERGE-001" Status set "Archived"
	When transaction "TRN-MERGE-001" is completed
	Then Cancelled by "TRN-MERGE-001" 2 "Occupancy" rights
	And Party "Mark Twen (merger)" have active "Occupancy" rights on "Property 5 Merge Result"
	And Property "Property 4 Merge 01" is archived
	And Property "Property 4 Merge 02" is archived

Scenario: Amalgamation merge of 2 properties
	This kinds operates with SU lifespan.
	It has 2 source properties and produce single target. Target is one of the source
	At this example only single RRR type assumed
	#precondition part reg 2 prop
	Given Registration transaction "REGO" with No."TRN-Src-AMALGA-MERGE-001" is set current
	And Current transaction has target LAUnit "Occupancy4A-Merge-LA-01"
	And Current transaction has property with Uid = "Property 7 A-Merge 01" in LAUnit "Occupancy4A-Merge-LA-01"
	And Current transaction has party "Robert Taler" with role "Grantee" associated with LAUnit "Occupancy4A-Merge-LA-01"
	#and pre data for 2nd prop
	Given Registration transaction "REGO" with No."TRN-Src-AMALGA-MERGE-002" is set current
	And Current transaction has target LAUnit "Occupancy4A-Merge-LA-02"
	And Current transaction has property with Uid = "Property 7 A-Merge 02" in LAUnit "Occupancy4A-Merge-LA-02"
	And Current transaction has party "John Adams" with role "Grantee" associated with LAUnit "Occupancy4A-Merge-LA-02"	
	When transaction "TRN-Src-AMALGA-MERGE-001" is completed
	And transaction "TRN-Src-AMALGA-MERGE-002" is completed

	Given Registration transaction "MERG" with No."TRN-A-MERGE-002" is set current
	And Current transaction has target LAUnit "Amalg-Merge-LA-01"
	#lets assume 1st property will be put to target
	And Current transaction has property with Uid = "Property 7 A-Merge 01" in LAUnit "Amalg-Merge-LA-01"
	And Current transaction has party "Matt Tomson (merger)" with role "Grantee" associated with LAUnit "Amalg-Merge-LA-01"	
	# we don't need for operational reasons source LAUnit, but it makes simple to operate within the same Transaction instance having LAUnit properties collection 
	And Current transaction has source LAUnit "Source 4 Amalg-Merge-LA-01"
	And Current transaction has property with Uid = "Property 7 A-Merge 01" in LAUnit "Source 4 Amalg-Merge-LA-01"
	And Current transaction has property with Uid = "Property 7 A-Merge 02" in LAUnit "Source 4 Amalg-Merge-LA-01"
	# in this case we chosen 1st to survive
	And Property "Property 7 A-Merge 02" in Transaction No"TRN-A-MERGE-002" Status set "Archived"
	When transaction "TRN-A-MERGE-002" is completed
	Then Cancelled by "TRN-A-MERGE-002" 2 "Occupancy" rights
	And Party "Matt Tomson (merger)" have active "Occupancy" rights on "Property 7 A-Merge 01"
	And Property "Property 7 A-Merge 01" is not archived
	And Property "Property 7 A-Merge 02" is archived
