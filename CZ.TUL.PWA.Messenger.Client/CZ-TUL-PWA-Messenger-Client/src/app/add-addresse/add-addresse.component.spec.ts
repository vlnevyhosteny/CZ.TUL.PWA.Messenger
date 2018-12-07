import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddAddresseComponent } from './add-addresse.component';

describe('AddAddresseComponent', () => {
  let component: AddAddresseComponent;
  let fixture: ComponentFixture<AddAddresseComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddAddresseComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddAddresseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
