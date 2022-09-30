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


//=============================================================================

#region H5public.h

/*
 * Status return values.  Failed integer functions in HDF5 result almost
 * always in a negative value (unsigned failing functions sometimes return
 * zero for failure) while successfull return is non-negative (often zero).
 * The negative failure value is most commonly -1, but don't bet on it.  The
 * proper way to detect failure is something like:
 *
 * 	if((dset = H5Dopen2(file, name)) < 0)
 *	    fprintf(stderr, "unable to open the requested dataset\n");
 */


/*
 * Boolean type.  Successful return values are zero (false) or positive
 * (true). The typical true value is 1 but don't bet on it.  Boolean
 * functions cannot fail.  Functions that return `htri_t' however return zero
 * (false), positive (true), or negative (failure). The proper way to test
 * for truth from a htri_t function is:
 *
 * 	if ((retval = H5Tcommitted(type))>0) {
 *	    printf("data type is committed\n");
 *	} else if (!retval) {
 * 	    printf("data type is not committed\n");
 *	} else {
 * 	    printf("error determining whether data type is committed\n");
 *	}
 */


/*
 * The sizes of file objects have their own types defined here, use a 64-bit
 * type.
 */


/*
 * File addresses have their own types.
 */


#endregion

//=============================================================================

#region H5Ipublic.h



#endregion

//=============================================================================

#region H5Opublic.h

/* Typedef for message creation indexes */

#endregion