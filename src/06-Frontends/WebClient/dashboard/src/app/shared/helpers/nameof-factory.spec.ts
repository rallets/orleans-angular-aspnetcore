import { TestBed, inject } from '@angular/core/testing';

import { nameofFactory } from '@app/shared/helpers/nameof-factory';

describe('NameofFactory', () => {

    class Aclass {
        pAr1: string;
        Par2: string;
    }

    beforeEach(() => {
        TestBed.configureTestingModule({
        });
    });

    it('should return a valid parameter', () => {
        const nameof = nameofFactory<Aclass>();

        const a = new Aclass();
        const v = nameof('pAr1');
        expect(v).toBe('pAr1');
    });

    it('should return a valid parameter to-lowercase', () => {
        const nameof = nameofFactory<Aclass>();

        const a = new Aclass();
        const v = nameof('pAr1', true);
        expect(v).toBe('par1');
    });

    it('should return a valid parameter NOT to-lowercase', () => {
        const nameof = nameofFactory<Aclass>();

        const a = new Aclass();
        const v = nameof('pAr1', false);
        expect(v).toBe('pAr1');
    });

});
