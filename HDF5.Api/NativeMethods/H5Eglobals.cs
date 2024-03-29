/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright by The HDF Group.                                               *
 * Copyright by the Board of Trustees of the University of Illinois.         *
 * All rights reserved.                                                      *
 *                                                                           *
 * This file is part of HDF5.  The full HDF5 copyright notice, including     *
 * terms governing use, modification, and redistribution, is contained in    *
 * the files COPYING and Copyright.html.  COPYING can be found at the root   *
 * of the source code distribution tree; Copyright.html can be found at the  *
 * root level of an installed copy of the electronic HDF5 document set and   *
 * is linked from the top-level documents page.  It can also be found at     *
 * http://hdfgroup.org/HDF5/doc/Copyright.html.  If you do not have          *
 * access to either file, you may request a copy from help@hdfgroup.org.     *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

namespace HDF5.Api.NativeMethods;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
internal static partial class H5E
{
    public static hid_t EARRAY { get; } = H5DLLImporter.Instance.GetHid("H5E_EARRAY_g");

    public static hid_t FARRAY { get; } = H5DLLImporter.Instance.GetHid("H5E_FARRAY_g");

    public static hid_t CANTDEPEND { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTDEPEND_g");

    public static hid_t CANTUNDEPEND { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTUNDEPEND_g");

    public static hid_t CANTNOTIFY { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTNOTIFY_g");

    public static hid_t LOGFAIL { get; } = H5DLLImporter.Instance.GetHid("H5E_LOGFAIL_g");

    public static hid_t CANTCORK { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTCORK_g");

    public static hid_t CANTUNCORK { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTUNCORK_g");

    public static hid_t CANTAPPEND { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTAPPEND_g");

    public static hid_t ERR_CLS { get; } = H5DLLImporter.Instance.GetHid("H5E_ERR_CLS_g");

    public static hid_t DATASET { get; } = H5DLLImporter.Instance.GetHid("H5E_DATASET_g");

    public static hid_t FUNC { get; } = H5DLLImporter.Instance.GetHid("H5E_FUNC_g");

    public static hid_t STORAGE { get; } = H5DLLImporter.Instance.GetHid("H5E_STORAGE_g");

    public static hid_t FILE { get; } = H5DLLImporter.Instance.GetHid("H5E_FILE_g");

    public static hid_t SOHM { get; } = H5DLLImporter.Instance.GetHid("H5E_SOHM_g");

    public static hid_t SYM { get; } = H5DLLImporter.Instance.GetHid("H5E_SYM_g");

    public static hid_t PLUGIN { get; } = H5DLLImporter.Instance.GetHid("H5E_PLUGIN_g");

    public static hid_t VFL { get; } = H5DLLImporter.Instance.GetHid("H5E_VFL_g");

    public static hid_t INTERNAL { get; } = H5DLLImporter.Instance.GetHid("H5E_INTERNAL_g");

    public static hid_t BTREE { get; } = H5DLLImporter.Instance.GetHid("H5E_BTREE_g");

    public static hid_t REFERENCE { get; } = H5DLLImporter.Instance.GetHid("H5E_REFERENCE_g");

    public static hid_t DATASPACE { get; } = H5DLLImporter.Instance.GetHid("H5E_DATASPACE_g");

    public static hid_t RESOURCE { get; } = H5DLLImporter.Instance.GetHid("H5E_RESOURCE_g");

    public static hid_t PLIST { get; } = H5DLLImporter.Instance.GetHid("H5E_PLIST_g");

    public static hid_t LINK { get; } = H5DLLImporter.Instance.GetHid("H5E_LINK_g");

    public static hid_t DATATYPE { get; } = H5DLLImporter.Instance.GetHid("H5E_DATATYPE_g");

    public static hid_t RS { get; } = H5DLLImporter.Instance.GetHid("H5E_RS_g");

    public static hid_t HEAP { get; } = H5DLLImporter.Instance.GetHid("H5E_HEAP_g");

    public static hid_t OHDR { get; } = H5DLLImporter.Instance.GetHid("H5E_OHDR_g");

    public static hid_t ATOM { get; } = H5DLLImporter.Instance.GetHid("H5E_ATOM_g");

    public static hid_t ATTR { get; } = H5DLLImporter.Instance.GetHid("H5E_ATTR_g");

    public static hid_t NONE_MAJOR { get; } = H5DLLImporter.Instance.GetHid("H5E_NONE_MAJOR_g");

    public static hid_t IO { get; } = H5DLLImporter.Instance.GetHid("H5E_IO_g");

    public static hid_t SLIST { get; } = H5DLLImporter.Instance.GetHid("H5E_SLIST_g");

    public static hid_t EFL { get; } = H5DLLImporter.Instance.GetHid("H5E_EFL_g");

    public static hid_t TST { get; } = H5DLLImporter.Instance.GetHid("H5E_TST_g");

    public static hid_t ARGS { get; } = H5DLLImporter.Instance.GetHid("H5E_ARGS_g");

    public static hid_t ERROR { get; } = H5DLLImporter.Instance.GetHid("H5E_ERROR_g");

    public static hid_t PLINE { get; } = H5DLLImporter.Instance.GetHid("H5E_PLINE_g");

    public static hid_t FSPACE { get; } = H5DLLImporter.Instance.GetHid("H5E_FSPACE_g");

    public static hid_t CACHE { get; } = H5DLLImporter.Instance.GetHid("H5E_CACHE_g");

    public static hid_t SEEKERROR { get; } = H5DLLImporter.Instance.GetHid("H5E_SEEKERROR_g");

    public static hid_t READERROR { get; } = H5DLLImporter.Instance.GetHid("H5E_READERROR_g");

    public static hid_t WRITEERROR { get; } = H5DLLImporter.Instance.GetHid("H5E_WRITEERROR_g");

    public static hid_t CLOSEERROR { get; } = H5DLLImporter.Instance.GetHid("H5E_CLOSEERROR_g");

    public static hid_t OVERFLOW { get; } = H5DLLImporter.Instance.GetHid("H5E_OVERFLOW_g");

    public static hid_t FCNTL { get; } = H5DLLImporter.Instance.GetHid("H5E_FCNTL_g");

    public static hid_t NOSPACE { get; } = H5DLLImporter.Instance.GetHid("H5E_NOSPACE_g");

    public static hid_t CANTALLOC { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTALLOC_g");

    public static hid_t CANTCOPY { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTCOPY_g");

    public static hid_t CANTFREE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTFREE_g");

    public static hid_t ALREADYEXISTS { get; } = H5DLLImporter.Instance.GetHid("H5E_ALREADYEXISTS_g");

    public static hid_t CANTLOCK { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTLOCK_g");

    public static hid_t CANTUNLOCK { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTUNLOCK_g");

    public static hid_t CANTGC { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTGC_g");

    public static hid_t CANTGETSIZE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTGETSIZE_g");

    public static hid_t OBJOPEN { get; } = H5DLLImporter.Instance.GetHid("H5E_OBJOPEN_g");

    public static hid_t CANTRESTORE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTRESTORE_g");

    public static hid_t CANTCOMPUTE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTCOMPUTE_g");

    public static hid_t CANTEXTEND { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTEXTEND_g");

    public static hid_t CANTATTACH { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTATTACH_g");

    public static hid_t CANTUPDATE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTUPDATE_g");

    public static hid_t CANTOPERATE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTOPERATE_g");

    public static hid_t CANTINIT { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTINIT_g");

    public static hid_t ALREADYINIT { get; } = H5DLLImporter.Instance.GetHid("H5E_ALREADYINIT_g");

    public static hid_t CANTRELEASE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTRELEASE_g");

    public static hid_t CANTGET { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTGET_g");

    public static hid_t CANTSET { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTSET_g");

    public static hid_t DUPCLASS { get; } = H5DLLImporter.Instance.GetHid("H5E_DUPCLASS_g");

    public static hid_t SETDISALLOWED { get; } = H5DLLImporter.Instance.GetHid("H5E_SETDISALLOWED_g");

    public static hid_t CANTMERGE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTMERGE_g");

    public static hid_t CANTREVIVE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTREVIVE_g");

    public static hid_t CANTSHRINK { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTSHRINK_g");

    public static hid_t LINKCOUNT { get; } = H5DLLImporter.Instance.GetHid("H5E_LINKCOUNT_g");

    public static hid_t VERSION { get; } = H5DLLImporter.Instance.GetHid("H5E_VERSION_g");

    public static hid_t ALIGNMENT { get; } = H5DLLImporter.Instance.GetHid("H5E_ALIGNMENT_g");

    public static hid_t BADMESG { get; } = H5DLLImporter.Instance.GetHid("H5E_BADMESG_g");

    public static hid_t CANTDELETE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTDELETE_g");

    public static hid_t BADITER { get; } = H5DLLImporter.Instance.GetHid("H5E_BADITER_g");

    public static hid_t CANTPACK { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTPACK_g");

    public static hid_t CANTRESET { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTRESET_g");

    public static hid_t CANTRENAME { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTRENAME_g");

    public static hid_t SYSERRSTR { get; } = H5DLLImporter.Instance.GetHid("H5E_SYSERRSTR_g");

    public static hid_t NOFILTER { get; } = H5DLLImporter.Instance.GetHid("H5E_NOFILTER_g");

    public static hid_t CALLBACK { get; } = H5DLLImporter.Instance.GetHid("H5E_CALLBACK_g");

    public static hid_t CANAPPLY { get; } = H5DLLImporter.Instance.GetHid("H5E_CANAPPLY_g");

    public static hid_t SETLOCAL { get; } = H5DLLImporter.Instance.GetHid("H5E_SETLOCAL_g");

    public static hid_t NOENCODER { get; } = H5DLLImporter.Instance.GetHid("H5E_NOENCODER_g");

    public static hid_t CANTFILTER { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTFILTER_g");

    public static hid_t CANTOPENOBJ { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTOPENOBJ_g");

    public static hid_t CANTCLOSEOBJ { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTCLOSEOBJ_g");

    public static hid_t COMPLEN { get; } = H5DLLImporter.Instance.GetHid("H5E_COMPLEN_g");

    public static hid_t PATH { get; } = H5DLLImporter.Instance.GetHid("H5E_PATH_g");

    public static hid_t NONE_MINOR { get; } = H5DLLImporter.Instance.GetHid("H5E_NONE_MINOR_g");

    public static hid_t OPENERROR { get; } = H5DLLImporter.Instance.GetHid("H5E_OPENERROR_g");

    public static hid_t FILEEXISTS { get; } = H5DLLImporter.Instance.GetHid("H5E_FILEEXISTS_g");

    public static hid_t FILEOPEN { get; } = H5DLLImporter.Instance.GetHid("H5E_FILEOPEN_g");

    public static hid_t CANTCREATE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTCREATE_g");

    public static hid_t CANTOPENFILE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTOPENFILE_g");

    public static hid_t CANTCLOSEFILE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTCLOSEFILE_g");

    public static hid_t NOTHDF5 { get; } = H5DLLImporter.Instance.GetHid("H5E_NOTHDF5_g");

    public static hid_t BADFILE { get; } = H5DLLImporter.Instance.GetHid("H5E_BADFILE_g");

    public static hid_t TRUNCATED { get; } = H5DLLImporter.Instance.GetHid("H5E_TRUNCATED_g");

    public static hid_t MOUNT { get; } = H5DLLImporter.Instance.GetHid("H5E_MOUNT_g");

    public static hid_t BADATOM { get; } = H5DLLImporter.Instance.GetHid("H5E_BADATOM_g");

    public static hid_t BADGROUP { get; } = H5DLLImporter.Instance.GetHid("H5E_BADGROUP_g");

    public static hid_t CANTREGISTER { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTREGISTER_g");

    public static hid_t CANTINC { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTINC_g");

    public static hid_t CANTDEC { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTDEC_g");

    public static hid_t NOIDS { get; } = H5DLLImporter.Instance.GetHid("H5E_NOIDS_g");

    public static hid_t CANTFLUSH { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTFLUSH_g");

    public static hid_t CANTSERIALIZE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTSERIALIZE_g");

    public static hid_t CANTLOAD { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTLOAD_g");

    public static hid_t PROTECT { get; } = H5DLLImporter.Instance.GetHid("H5E_PROTECT_g");

    public static hid_t NOTCACHED { get; } = H5DLLImporter.Instance.GetHid("H5E_NOTCACHED_g");

    public static hid_t SYSTEM { get; } = H5DLLImporter.Instance.GetHid("H5E_SYSTEM_g");

    public static hid_t CANTINS { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTINS_g");

    public static hid_t CANTPROTECT { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTPROTECT_g");

    public static hid_t CANTUNPROTECT { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTUNPROTECT_g");

    public static hid_t CANTPIN { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTPIN_g");

    public static hid_t CANTUNPIN { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTUNPIN_g");

    public static hid_t CANTMARKDIRTY { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTMARKDIRTY_g");

    public static hid_t CANTDIRTY { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTDIRTY_g");

    public static hid_t CANTEXPUNGE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTEXPUNGE_g");

    public static hid_t CANTRESIZE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTRESIZE_g");

    public static hid_t TRAVERSE { get; } = H5DLLImporter.Instance.GetHid("H5E_TRAVERSE_g");

    public static hid_t NLINKS { get; } = H5DLLImporter.Instance.GetHid("H5E_NLINKS_g");

    public static hid_t NOTREGISTERED { get; } = H5DLLImporter.Instance.GetHid("H5E_NOTREGISTERED_g");

    public static hid_t CANTMOVE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTMOVE_g");

    public static hid_t CANTSORT { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTSORT_g");

    public static hid_t MPI { get; } = H5DLLImporter.Instance.GetHid("H5E_MPI_g");

    public static hid_t MPIERRSTR { get; } = H5DLLImporter.Instance.GetHid("H5E_MPIERRSTR_g");

    public static hid_t CANTRECV { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTRECV_g");

    public static hid_t CANTCLIP { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTCLIP_g");

    public static hid_t CANTCOUNT { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTCOUNT_g");

    public static hid_t CANTSELECT { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTSELECT_g");

    public static hid_t CANTNEXT { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTNEXT_g");

    public static hid_t BADSELECT { get; } = H5DLLImporter.Instance.GetHid("H5E_BADSELECT_g");

    public static hid_t CANTCOMPARE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTCOMPARE_g");

    public static hid_t UNINITIALIZED { get; } = H5DLLImporter.Instance.GetHid("H5E_UNINITIALIZED_g");

    public static hid_t UNSUPPORTED { get; } = H5DLLImporter.Instance.GetHid("H5E_UNSUPPORTED_g");

    public static hid_t BADTYPE { get; } = H5DLLImporter.Instance.GetHid("H5E_BADTYPE_g");

    public static hid_t BADRANGE { get; } = H5DLLImporter.Instance.GetHid("H5E_BADRANGE_g");

    public static hid_t BADVALUE { get; } = H5DLLImporter.Instance.GetHid("H5E_BADVALUE_g");

    public static hid_t NOTFOUND { get; } = H5DLLImporter.Instance.GetHid("H5E_NOTFOUND_g");

    public static hid_t EXISTS { get; } = H5DLLImporter.Instance.GetHid("H5E_EXISTS_g");

    public static hid_t CANTENCODE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTENCODE_g");

    public static hid_t CANTDECODE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTDECODE_g");

    public static hid_t CANTSPLIT { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTSPLIT_g");

    public static hid_t CANTREDISTRIBUTE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTREDISTRIBUTE_g");

    public static hid_t CANTSWAP { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTSWAP_g");

    public static hid_t CANTINSERT { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTINSERT_g");

    public static hid_t CANTLIST { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTLIST_g");

    public static hid_t CANTMODIFY { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTMODIFY_g");

    public static hid_t CANTREMOVE { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTREMOVE_g");

    public static hid_t CANTCONVERT { get; } = H5DLLImporter.Instance.GetHid("H5E_CANTCONVERT_g");

    public static hid_t BADSIZE { get; } = H5DLLImporter.Instance.GetHid("H5E_BADSIZE_g");
}
