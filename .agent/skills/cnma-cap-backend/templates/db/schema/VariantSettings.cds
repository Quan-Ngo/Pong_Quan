namespace cnma.adminconfiguration;

using {
    managed,
    cuid
} from '@sap/cds/common';

@cds.persistence.exists
entity VariantSettings : cuid, managed {
    variantKey         : String not null;
    variantName        : String not null;
    workListId         : String not null;
    isDefaultVariant   : Boolean default false;
    isGlobalVariant    : Boolean default false; // not use anymore due to complexity logic
    filterBarVariant   : String;
    filterDataVariant  : String;
    tableColumnVariant : String;
    tableSortVariant   : String;
    tableGroupVariant  : String;
}
