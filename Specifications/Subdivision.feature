Feature: Subdivision
		It happened when single or some set of source properties produces new set
		It may vary in results depends on lifespan of origin 
		whethere it (o)=>{o`,n} or (o)=>{n1,n2} or {o1,o2} => {o1`,o2`,n...}
		But main idea - cancel all RRR on source and produce new for targets
		We may work on LAUnits for different right types at the same operation

Background: 

@subdivision
Scenario: Subdivision produces 2 new properties and no additional interests registered on source
		  Source property is archived as a result. Split.

#PRE
	Given We have Parcel with Uid = "Property 2"
	#/// REGO - Registration of Occupancy right to Grantee - see Initializer for details	 
	And Registration transaction "REGO" with No."TRN-PRE-OCCUPANCY-4-SUBD-002" is set current
	#/// LAUnits represents interests to be registered proto
	And Current transaction has target LAUnit "LA-Occupancy-02"
	And Current transaction has property with Uid = "Property 2" in LAUnit "LA-Occupancy-02"
	And Current transaction has party "Tim Taler" with role "Grantee" associated with LAUnit "LA-Occupancy-02"
	And Current transaction has party "Sara Taler" with role "Grantee" associated with LAUnit "LA-Occupancy-02"
	# same result as prev both
	#And Current transaction target parties reference property with Uid = "Property 2"
	When transaction "TRN-PRE-OCCUPANCY-4-SUBD-002" is completed

#MAIN
	Given Registration transaction "SUBS" with No."TRN-SUBS-003" is set current
	And Current transaction has source LAUnit "LA-Occupancy-02"
	And Current transaction has target LAUnit "LA-Occupancy-03"
	And Current transaction has target LAUnit "LA-Occupancy-04"
	And Current transaction has property with Uid = "Property 2" in LAUnit "LA-Occupancy-02"
	#user may decide does it survive or not, server will obey only
	And Property "Property 2" in Transaction No"TRN-SUBS-003" Status set "Archived"
	And Current transaction has property with Uid = "Property 3" in LAUnit "LA-Occupancy-03"
	And Current transaction has property with Uid = "Property 4" in LAUnit "LA-Occupancy-04"
	And Current transaction has party "Tim Taler" with role "Grantee" associated with LAUnit "LA-Occupancy-03"
	And Current transaction has party "Sara Taler" with role "Grantee" associated with LAUnit "LA-Occupancy-04"
	When transaction "TRN-SUBS-003" is completed
	Then Cancelled by "TRN-SUBS-003" 2 "Occupancy" rights
	And Party "Tim Taler" have active "Occupancy" rights on "Property 3"
	And Party "Sara Taler" have active "Occupancy" rights on "Property 4"
	And Property "Property 2" is archived

Scenario: Property changed with part of it registered as new property
	For example in case Park is reduced and new parcel set up
	The idea there to preserve interests and prev cadaster element
#PRE
	Given We have Parcel with Uid = "Property: Park"
	#/// REGO - Registration of Occupancy right to Grantee - see Initializer for details	 
	And Registration transaction "REGO" with No."TRN-Park 4 Sub - 01" is set current
	#/// LAUnits represents interests to be registered proto
	And Current transaction has target LAUnit "LA-Occupancy-04-003"
	And Current transaction has property with Uid = "Property: Park" in LAUnit "LA-Occupancy-04-003"
	And Current transaction has party "City Community" with role "Grantee" associated with LAUnit "LA-Occupancy-04-003"
	When transaction "TRN-Park 4 Sub - 01" is completed
#MAIN
	Given Registration transaction "SUBS" with No."TRN-SUBS-Clear-001" is set current
	And Current transaction has source LAUnit "LA-Occupancy-04-003"
	And Current transaction has target LAUnit "LA-Occupancy-04-003"
	And Current transaction has target LAUnit "LA-Occupancy-04-003-New"
	And Current transaction has property with Uid = "Property 2" in LAUnit "LA-Occupancy-02"
	#user may decide does it survive or not, server will obey only
	And Property "Property 2" in Transaction No"TRN-SUBS-003" Status set "Archived"
	And Current transaction has property with Uid = "Property 3" in LAUnit "LA-Occupancy-03"
	And Current transaction has property with Uid = "Property 4" in LAUnit "LA-Occupancy-04"
	And Current transaction has party "Tim Taler" with role "Grantee" associated with LAUnit "LA-Occupancy-03"
	And Current transaction has party "Sara Taler" with role "Grantee" associated with LAUnit "LA-Occupancy-04"
	When transaction "TRN-SUBS-003" is completed
	Then Cancelled by "TRN-SUBS-003" 2 "Occupancy" rights
	And Party "Tim Taler" have active "Occupancy" rights on "Property 3"
	And Party "Sara Taler" have active "Occupancy" rights on "Property 4"
	And Property "Property 2" is archived
	

